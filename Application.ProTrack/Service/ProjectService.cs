using Application.ProTrack.DTO.ProjectDto;
using Application.ProTrack.Service.Interface;
using Domain.ProTrack.Models;
using Domain.ProTrack.RepoInterface;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.ProTrack.DTO;
using static Domain.ProTrack.Enum.Enum;

namespace Application.ProTrack.Service
{
    public class ProjectService : IProjectServiceInterface
    {
        private readonly IProjectRepoInterface _projectRepo;
        private readonly ILogger<IProjectServiceInterface> _logger;
        private readonly IEmailDispatcherServiceInterface _emailDispatcherService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserServiceInterface _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectHelperService _projectHelperService;
        private readonly IEmailNotificationHelperInterface _emailNotificationHelper;
        public ProjectService(
            IProjectRepoInterface projectRepo,
            ILogger<IProjectServiceInterface> logger,
            UserManager<AppUser> userManager, 
            IUserServiceInterface userService, 
            IEmailDispatcherServiceInterface emailDispatcherService,
            IUnitOfWork unitOfWork,
            IProjectHelperService projectHelperService,
            IEmailNotificationHelperInterface emailNotificationHelper)
        {
            _projectRepo = projectRepo;
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
            _emailDispatcherService = emailDispatcherService;
            _unitOfWork = unitOfWork;
            _projectHelperService = projectHelperService;
            _emailNotificationHelper = emailNotificationHelper;
        }
        public async Task<IdentityResult> CreateProjectAsync(CreateProjectDto createProject)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var manager = await _projectHelperService.FindManagerInfo(createProject.ManagerUsername)
                    ?? throw new KeyNotFoundException("Manager not found");

                if(createProject.StartDate > createProject.EndDate)
                {
                    throw new InvalidOperationException("Logic Error! End date cannot be less than start date");
                }
                var projectModel = new Project
                {
                    ProjectManagerId = manager,
                    Title = createProject.Title,
                    ProjectDescription = createProject.ProjectDescription,
                    StartDate = createProject.StartDate,
                    EndDate = createProject.EndDate,
                    Status = Status.Pending,
                    CreatedBy = "Admin",
                    Priority = createProject.Priority,
                };
                var projectUserModel = new List<ProjectUser>();

                //add manager
                projectUserModel.Add(new ProjectUser
                {
                    ProjectId = projectModel.ProjectId,
                    AssignedUserId = manager,
                    UserRole = UserRole.ProjectManager
                });

                //adding members
                var allMembers = await _projectHelperService.FindMemberInfo(createProject.MembersUsername);
                var nonManagerMembers = allMembers.Where(member => member != manager).ToHashSet();
                projectUserModel.AddRange(nonManagerMembers.Select(member =>  new ProjectUser
                {
                    ProjectId = projectModel.ProjectId,
                    AssignedUserId = member,
                    UserRole = UserRole.Member
                }).ToList());

                var result = await _projectRepo.CreateProjectAsync(projectModel, projectUserModel);
                if (result)
                {
                    await _userService.AssignRoleToEmployee(manager, allMembers);
                    //enqueing email jobs
                    _emailNotificationHelper.QueueProjectTaskCreationEmails(manager, nonManagerMembers, createProject.Title, null,null);

                    await _unitOfWork.SaveChangesAsync();

                    await _unitOfWork.CommitTransactionAsync();

                    await _emailDispatcherService.DispatchAsync();
                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "ProjectNotCreated",
                    Description = "Failed to create project"
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransactionAsync();
                _logger.LogError(ex, "Failed to create project '{Title}' having manager '{Manager}'", createProject.Title, createProject.ManagerUsername);
                throw new InvalidOperationException($"Unexpected Error! Failed to create project",ex);
            }
        }
        public async Task<GetProjectDetailsDto> GetProjectDetails(Guid projectId)
        {
            try
            {
                return await _projectRepo.GetProjectDetails(projectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error! Failed to get the user details");
                throw new ApplicationException("Internal Server Error! Failed to get project details",ex);
            }
        }
        public async Task<IdentityResult> UpdateProject(UpdateProjectDto updateProject, Guid projectId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var currentUser = await _userService.GetCurrentUser();
                var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
                var incomingManagerId = await _projectHelperService.FindManagerInfo(updateProject.ManagerUsername) 
                    ?? throw new InvalidOperationException("Project Manager dosent exist");
                var incomingMangerIdSet = new HashSet<string> { incomingManagerId };
                var existingProject = await _projectRepo.GetProjectAsync(projectId);
                var initialManagerId = await _projectRepo.GetExistingProjectManagerIds(projectId);
                var initialManagerIdSet = new HashSet<string> { initialManagerId };
                var incomingMembersIds = await _projectHelperService.FindMemberInfo(updateProject.MembersUsername);
                var initialMemberIds = await _projectRepo.GetExistingProjectMemberIds(projectId);
  
                // Remove members that were previously assigned but are no longer in the incoming list
                var memberToRemoveIds = new HashSet<string>(initialMemberIds);
                memberToRemoveIds.ExceptWith(incomingMembersIds);
                if(memberToRemoveIds.Any())
                {
                    await _projectHelperService.RemoveMemberFromProject(memberToRemoveIds, incomingMembersIds, projectId, incomingManagerId);
                }
                // List of ProjectUser objects to be added to the project
                var projectUserModel = new List<ProjectUser>();
                // Replace the existing project manager if a different one is assigned
                if (initialManagerId != incomingManagerId)
                {
                    if (isAdmin)
                    {
                        var removed = await _projectHelperService.RemoveManagerFromProject(initialManagerId, projectId, incomingMembersIds);
                        if (removed)
                        {
                            projectUserModel.Add(new ProjectUser
                            {
                                ProjectId = projectId,
                                AssignedUserId = incomingManagerId,
                                UserRole = UserRole.ProjectManager,
                            });
                            existingProject.ProjectManagerId = incomingManagerId;
                            existingProject.UpdatedDate = DateTime.UtcNow;
                            //queuing manager changed email notification
                            var membersToSendMail = initialMemberIds.Except(incomingMangerIdSet).Except(memberToRemoveIds).ToHashSet();
                            _emailNotificationHelper.QueueManagerChangedEmail(membersToSendMail, existingProject.Title, incomingManagerId, null, null);
                        }
                    }
                }
                bool hasChanged = _projectHelperService.HasProjectChanged(existingProject, updateProject);
                //updating project
                if (hasChanged)
                {
                    existingProject.Title = updateProject.Title;
                    existingProject.ProjectDescription = updateProject.ProjectDescription;
                    existingProject.StartDate = updateProject.StartDate;
                    existingProject.EndDate = updateProject.EndDate;
                    existingProject.UpdatedDate = DateTime.UtcNow;
                    existingProject.UpdatedBy = currentUser.UserName;
                    existingProject.Priority = updateProject.Priority;
                    existingProject.Status = updateProject.Status;
                }
                // Identify and add new members who are in the incoming list but not already assigned
                incomingMembersIds.ExceptWith(initialMemberIds);
                var newMembersToAdd = incomingMembersIds;
                //checking if changes were made and if not early exit
                bool shouldSkipUpdate = _projectHelperService.ShouldSkipProjectUpdate(incomingManagerId, initialManagerId, newMembersToAdd, hasChanged);
                if (shouldSkipUpdate)
                {
                    _logger.LogInformation("No updates for project '{ProjectId}'. Skipped because manager unchanged, no member diff, and title unchanged.", projectId);
                    return IdentityResult.Success;
                }
                //adding the new members
                if (newMembersToAdd.Any())
                {
                    if (isAdmin)
                    {
                        projectUserModel.AddRange(newMembersToAdd.Select(member => new ProjectUser
                        {
                            ProjectId = projectId,
                            AssignedUserId = member,
                            UserRole = incomingManagerId == member ? UserRole.ProjectManager : UserRole.Member,
                        }).ToList());
                        var membersToSendEmail = newMembersToAdd.Except(initialManagerIdSet).ToHashSet();

                        //Queue newly added memebers mail notification
                        _emailNotificationHelper.QueueNewlyAddedMembersEmail(membersToSendEmail, incomingManagerId, existingProject.Title, null, null);
                    }
                }
                //updating project
                var result = await _projectRepo.UpdateProjectWithMembersAsync(existingProject,projectUserModel);
                if(result && projectUserModel.Any())
                {
                    await _userService.AssignRoleToEmployee(incomingManagerId, incomingMembersIds);
                }
                await _unitOfWork.CommitTransactionAsync();
                await _unitOfWork.SaveChangesAsync();
                await _emailDispatcherService.DispatchAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransactionAsync();
                _logger.LogError(ex, "Failed to update project having id '{Id}' to '{Title}' with manager '{Manager}' and members '{Members}'",
                    projectId, updateProject.Title, updateProject.ManagerUsername, updateProject.MembersUsername  );

                throw new InvalidOperationException($"Unexpected Error! Failed to update project");
            }
        }
        public async Task<IdentityResult> RemoveProject(Guid projectId)
        {
            try
            {
                var projectToRemove = await _projectRepo.GetProjectAsync(projectId);
                var prjectMembersIds = await _projectRepo.GetAllProjectMembersIds(projectId);
                
                if (projectToRemove != null)
                {
                    //ressigning members as employees
                    foreach(var membersId in prjectMembersIds)
                    {
                        await _userService.ReassignToEmployeeRole(membersId);
                    }

                    await _projectRepo.DeleteProjectAsync(projectToRemove);
                    await _unitOfWork.SaveChangesAsync();
                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "ProjectNotRemoved",
                    Description = "Unexpeced error! Failed to delete project"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error! Failed to remove project '{projectId}'", projectId);
                throw new UnauthorizedAccessException($"Unexpected error! Manager not found");
            }
        }
        public async Task<List<Project>> GetAllProjectAsync()
        {
            try
            {
                return await _projectRepo.GetAllProjectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error! Failed to get all the project");
                throw new KeyNotFoundException($"Unexpected error! Couldnt fetch the projects");
            }

        }
    }
}

