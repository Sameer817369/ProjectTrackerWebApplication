using Application.ProTrack.DTO.TaskDto;
using Application.ProTrack.Service.Interface;
using Domain.ProTrack.Models;
using Domain.ProTrack.RepoInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.ProTrack.Dto;
using static Domain.ProTrack.Enum.Enum;

namespace Application.ProTrack.Service
{
    public class TaskService : ITaskServiceInterface
    {
        private readonly ITaskRepositoryInterface _taskRepo;
        private readonly ILogger<ITaskServiceInterface> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepoInterface _userRepo;
        private readonly IProjectRepoInterface _projectRepo;
        private readonly IUserServiceInterface _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskHelperServiceInterface _taskHelperService;
        private readonly IEmailNotificationHelperInterface _emailNotificationHelper;
        private readonly IEmailDispatcherServiceInterface _emailDispatcherService;
        public TaskService(ITaskRepositoryInterface taskRepo,
            ILogger<ITaskServiceInterface> logger, 
            IUserRepoInterface userRepo,
            IProjectRepoInterface projectRepo,
            UserManager<AppUser> userManager, 
            IUserServiceInterface userService,
            IUnitOfWork unitOfWork,
            ITaskHelperServiceInterface taskHelperService,
            IEmailNotificationHelperInterface emailNotificationHelper,
            IEmailDispatcherServiceInterface emailDispatcherService)
        {
            _taskRepo = taskRepo;
            _logger = logger;
            _userRepo = userRepo;
            _projectRepo = projectRepo;
            _userManager = userManager;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _emailNotificationHelper = emailNotificationHelper;
            _taskHelperService = taskHelperService;
            _emailDispatcherService = emailDispatcherService;
        }
        public async Task<IdentityResult> CreateTask(CreateTaskDto createTask, Guid projectId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var incomingTaskManager= await _userManager.FindByNameAsync(createTask.ManagerUsername)
                    ?? throw new InvalidOperationException("Manager not found");
                var projectMembers = await _projectRepo.GetProjectUsersAsync(projectId, createTask.MemberUsername, createTask.ManagerUsername) ?? throw new InvalidOperationException("Couldnt fetch members");
                var project = await _projectRepo.GetProjectAsync(projectId);
                var taksProject = await _projectRepo.GetProjectAsync(projectId);
                if (!projectMembers.Any(member => member.AssignedUserId == incomingTaskManager.Id))
                {
                    throw new InvalidOperationException("Incoming Task Manager not part of the project");
                }
                if (createTask.StartDate > createTask.EndDate || createTask.StartDate < taksProject.StartDate || createTask.EndDate > taksProject.EndDate)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "InvalidDateAssignment",
                        Description = "Date range must align with the project's timeline"
                    });
                }
                var taskModel = new Tasks
                {
                    ProjectId = projectId,
                    Title = createTask.Title,
                    Description = createTask.Description,
                    StartDate = createTask.StartDate,
                    EndDate = createTask.EndDate,
                    Status = Status.Pending,
                    TaskManagerId = incomingTaskManager.Id,
                    CreatedBy = "Project Manager",
                    Priority = createTask.Priority,
                };
                var projectTaskUserModel = projectMembers.Select(member => new ProjectUserTask
                {
                    ProjectUserId = member.ProjectUserId,
                    TaskId = taskModel.TaskId,
                    UserRole = member.AssignedUserId == incomingTaskManager.Id ? UserRole.TaskManager : UserRole.Member
                }).ToList();

                var result = await _taskRepo.CreateTaskAsync(taskModel, projectTaskUserModel);
                if (result)
                {
                    await _userManager.AddToRoleAsync(incomingTaskManager, "Task Manager");
                    //queuing emails
                    var membersToSendEmail = projectMembers.Where(u=> u.AssignedUserId != incomingTaskManager.Id).Select(u => u.AssignedUserId).ToHashSet();
                    _emailNotificationHelper.QueueProjectTaskCreationEmails(project.ProjectManagerId, membersToSendEmail, project.Title, incomingTaskManager.Id, createTask.Title);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                    await _emailDispatcherService.DispatchAsync();

                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "TaskNotCreated",
                    Description = "Failed to create task" 
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransactionAsync();
                _logger.LogError(ex, $"Unexpected Error! Failed to create taks for project {projectId}");
                throw new InvalidOperationException($"Unexpected Error! Failed to create task",ex);
            }
        }
        public async Task<GetTaskDetailsDto>GetTaskDetailsAsync(Guid projectId,Guid taskId)
        {
            try
            {
                var result = await _taskRepo.GetTaskDetails(projectId,taskId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected Error! Failed to get taks details");
                throw new InvalidOperationException($"Unexpected Error! Failed to get task", ex);
            }
        }

        public async Task<IdentityResult> UpdateTaskAsync(UpdateTaskDto updateTask, Guid projectId, Guid taskId)
         {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var currentUser = await _userService.GetCurrentUser();
                var isProjectManager = await _userManager.IsInRoleAsync(currentUser, "Project Manager");
                var existingTask = await _taskRepo.GetExistingTaskAsync(taskId, projectId)
                    ?? throw new KeyNotFoundException($"Task with id '{taskId}' not found");
                var taksProject = await _projectRepo.GetProjectAsync(projectId);
                var initialTaskManagerId = await _taskRepo.GetProjectUserTaskManager(taskId, projectId);
                var initialManagerIdSet = new HashSet<string> { initialTaskManagerId };
                var initialManager = await _userManager.FindByIdAsync(initialTaskManagerId);
                var incomingManager = await _userManager.FindByNameAsync(updateTask.managerUsername) 
                    ?? throw new KeyNotFoundException("Manager doesnot exist");
                var incomingManagerId = incomingManager.Id;
                var verifiedIncomingMembers = await _projectRepo.GetProjectUsersAsync(projectId, updateTask.MemberUsername, updateTask.managerUsername);
                var existingProjectMembersIds = await _projectRepo.GetExistingProjectMemberIds(projectId); 
                var exisitingTaskMembersIds = await _taskRepo.GetProjectUserTaskMembers(taskId);
                var verifiedIncomingMembersIds = verifiedIncomingMembers.Where(u=>u.AssignedUserId != incomingManagerId).Select(u => u.AssignedUserId).ToHashSet();
                var taskMembersToRemove = exisitingTaskMembersIds;
                taskMembersToRemove.ExceptWith(verifiedIncomingMembersIds);
                if (taskMembersToRemove.Any())
                {
                    await _taskHelperService.RemoveMemberFromTask(taskMembersToRemove, taskId, initialManager,incomingManager, projectId);
                }
                //Getting the current existing memebers after cleansing
                var currentTaskMembersIds = await _taskRepo.GetProjectUserTaskMembers(taskId);
                //new task members to be assgined to the task
                var newTaskMembers = verifiedIncomingMembers.Where(u => !currentTaskMembersIds.Contains(u.AssignedUserId) && u.AssignedUserId != incomingManagerId).ToList();

                var projectUserTaskModel = new List<ProjectUserTask>();
                // If the task manager has changed, remove the old one and assign the new manager
                if (initialTaskManagerId != incomingManagerId)
                {
                    if (isProjectManager)
                    {
                        await _taskHelperService.RemovePerviousManager(taskId, initialTaskManagerId, projectId, newTaskMembers.Select(u => u.AssignedUserId).ToHashSet());
                        var managerToAdd = verifiedIncomingMembers.FirstOrDefault(u => u.AssignedUserId == incomingManagerId);
                        var manager = await _userManager.FindByIdAsync(managerToAdd.AssignedUserId);
                        projectUserTaskModel.Add(new ProjectUserTask
                        {
                            ProjectUserId = managerToAdd.ProjectUserId,
                            TaskId = taskId,
                            UserRole = UserRole.TaskManager
                        });
                        existingTask.TaskManagerId = incomingManagerId;
                        existingTask.UpdatedDate = DateTime.UtcNow;
                        await _userManager.AddToRoleAsync(manager, "Task Manager");
            
                        //data to send in email
                        var currentProject = await _projectRepo.GetProjectAsync(projectId);
                        var membersToSendEmail = currentTaskMembersIds.Where(id => id != incomingManagerId).ToHashSet();
                        var projectTitle = currentProject.Title;
                        var projectManagerId = currentProject.ProjectManagerId;
                        var taskTitle = updateTask.Title;
                        _emailNotificationHelper.QueueManagerChangedEmail(membersToSendEmail, projectTitle,projectManagerId, incomingManagerId, taskTitle);
                    }
                }

                if (updateTask.StartDate > updateTask.EndDate || updateTask.StartDate < taksProject.StartDate || updateTask.EndDate > taksProject.EndDate)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "InvalidDateAssignment",
                        Description = "Date range must align with the project's timeline"
                    });
                }
                bool hasChanged = _taskHelperService.HasChanged(existingTask, updateTask);
                if (hasChanged)
                {
                    existingTask.Title = updateTask.Title;
                    existingTask.UpdatedDate = DateTime.UtcNow;
                    existingTask.StartDate = updateTask.StartDate;
                    existingTask.EndDate = updateTask.EndDate;
                    existingTask.Description = updateTask.Description;
                    existingTask.UpdatedBy = currentUser.UserName;
                    existingTask.Priority = updateTask.Priority;
                }
             
                if (newTaskMembers.Any())
                {
                    if(isProjectManager)
                    {
                        projectUserTaskModel.AddRange(newTaskMembers.Select(member => new ProjectUserTask
                        {
                            ProjectUserId = member.ProjectUserId,
                            TaskId = taskId,
                            UserRole = UserRole.Member
                        }).ToList());
                        //data to send in email
                        var membersToSendEmail = newTaskMembers.Select(u => u.AssignedUserId).ToHashSet();
                        membersToSendEmail.Except(initialManagerIdSet);
                        var currentProject = await _projectRepo.GetProjectAsync(projectId);
                        var projectTitle = currentProject.Title;
                        var projectManagerId = currentProject.ProjectManagerId;
                        var taskTitle = updateTask.Title;
                        _emailNotificationHelper.QueueNewlyAddedMembersEmail(membersToSendEmail, projectManagerId, projectTitle, incomingManagerId, taskTitle);
                    }
                }
                var shouldSkip = _taskHelperService.ShouldSkipTaskUpdate(incomingManagerId,initialTaskManagerId, hasChanged, newTaskMembers.Select(u=>u.AssignedUserId).ToHashSet());
                if (shouldSkip)
                {
                    _logger.LogInformation("No updates for task '{TaskId}'. Skipped because manager unchanged, no member diff, and title unchanged.", taskId);
                    return IdentityResult.Success;
                }
                var result = await _taskRepo.UpdateTaskAsync(existingTask, projectUserTaskModel);
                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                    await _emailDispatcherService.DispatchAsync();
                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError { Description = "Unexpected error! Failed to assign new members to the task." });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackTransactionAsync();
                _logger.LogError(ex,"Failed to update task '{TaskId}' in project '{ProjectId}'",taskId, projectId);
                throw new InvalidOperationException($"Unexpected Error! Failed to update task {ex.Message}");
            }
        }
        public async Task<IdentityResult> RemoveTask(Guid taskId, Guid projectId)
        {
            try
            {
                var taskToRemove = await _taskRepo.GetExistingTaskAsync(taskId, projectId);
                if (taskToRemove != null)
                {
                    await _taskRepo.DeleteTaskAsync(taskToRemove);
                    await _unitOfWork.SaveChangesAsync();
                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError { Description = "Unexpected Error! Failed to remove task" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error! Failed to remove task '{taskId}'", taskId);
                throw new InvalidOperationException($"Unexpected Error! Failed to remove task {ex.Message}");
            }
        }
    }
}
