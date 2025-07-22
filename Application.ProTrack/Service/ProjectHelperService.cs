using Application.ProTrack.DTO.ProjectDto;
using Application.ProTrack.Service.Interface;
using Domain.ProTrack.Models;
using Domain.ProTrack.RepoInterface;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Domain.ProTrack.Enum.Enum;

namespace Application.ProTrack.Service
{
    public class ProjectHelperService : IProjectHelperService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<IProjectHelperService> _logger;
        private readonly IUserServiceInterface _userService;
        private readonly IProjectRepoInterface _projectRepo;

        private readonly IEmailNotificationHelperInterface _projectEmailNotificationHelper;
        public ProjectHelperService(UserManager<AppUser> userManager, 
            ILogger<IProjectHelperService> logger, 
            IUserServiceInterface userService,
            IProjectRepoInterface projectRepo, 
            IEmailNotificationHelperInterface projectEmailNotificationHelper)
        {
            _userManager = userManager;
            _logger = logger;
            _userService = userService;
            _projectRepo = projectRepo;
            _projectEmailNotificationHelper = projectEmailNotificationHelper;
        }
        public async Task<string> FindManagerInfo(string managerUsername)
        {
            try
            {
                var manager = await _userManager.FindByNameAsync(managerUsername);
                if (manager == null) return ("Error! Manager is empty");
                return manager.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error! Failed to get manager info");
                throw new UnauthorizedAccessException($"Unexpected error! Manager not found");
            }
        }

        public async Task<HashSet<string>> FindMemberInfo(List<string> memberUsername)
        {
            try
            {
                var members = await _userManager.Users.Where(u => memberUsername.Contains(u.UserName)).ToHashSetAsync();
                var memberUsernameSet = memberUsername.ToHashSet();
                var foundMemebersUsernames = members.Select(u => u.UserName).ToHashSet();
                memberUsernameSet.ExceptWith(foundMemebersUsernames);
                if (memberUsernameSet.Any())
                {
                    _logger.LogWarning("Some usernames were not found: {Usernames}", string.Join(", ", memberUsernameSet));
                }
                return members.Select(u => u.Id).ToHashSet();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error! Failed to get member info");
                throw new InvalidOperationException($"Unexpected error! Members not found");
            }
        }

        public bool HasProjectChanged(Project existing, UpdateProjectDto dto)
        {
            return existing.Title != dto.Title ||
                  existing.ProjectDescription != dto.ProjectDescription ||
                  existing.StartDate != dto.StartDate ||
                  existing.EndDate != dto.EndDate ||
                  existing.Priority != dto.Priority ||
                  existing.Status != dto.Status ;
        }

        public async Task<bool> RemoveManagerFromProject(string existingManagerId, Guid projectId, HashSet<string> incomingMemberIds)
        {
            try
            {
                var isPresent = incomingMemberIds.Contains(existingManagerId);
                var userToRemove = await _projectRepo.GetManagerToRemove(projectId, existingManagerId);
                var result = await _projectRepo.RemoveProjectManagerAsync(userToRemove);
                if (result)
                {
                    await _userService.ReassignToEmployeeRole(existingManagerId);

                    var projectHistoryModel = new ProjectHistory
                    {
                        ProjectName = userToRemove.Project.Title,
                        ChangedByUser = "Admin",
                        ChangedByUserEmail = "Admin@gmail.com",
                        ChangedUser = userToRemove.AssignedUser.UserName,
                        ChangedUserEmail = userToRemove.AssignedUser.Email,
                        NewRole = isPresent ? "Member" : "Employee",
                        PreviousRole = "Project Manager",
                        ChangeType = isPresent ? Changed.Demoted : Changed.Removed,
                    };
                    if (projectHistoryModel != null)
                    {
                        _projectEmailNotificationHelper.QueueManagerRemovedEmail(existingManagerId, projectHistoryModel.ProjectName,null,null, null);
                        await _projectRepo.CreateProjectHistory(null, projectHistoryModel);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error! Failed to remove previous manager with id '{existingManager}' from project", existingManagerId);
                throw new KeyNotFoundException($"Unexpected error! Previous Manager not removed form the project");
            }
        }

        public async Task<bool> RemoveMemberFromProject(HashSet<string> exsitingMemberIds, HashSet<string> incomingMemberIdSets, Guid projectId, string incomingManangerId)
        {
            try
            {
                exsitingMemberIds.ExceptWith(incomingMemberIdSets);
                var idsToRemove = exsitingMemberIds.ToList();
                var userToRemove = await _projectRepo.GetMembersToRemove(projectId, idsToRemove)
                    ?? throw new InvalidOperationException("Members not found");
                var userToRemoveSetIds = userToRemove.Select(u => u.AssignedUserId).ToHashSet();

                if (userToRemove.Any())
                {
                    var result = await _projectRepo.RemoveProjectMembersAsync(userToRemove);
                    if (result)
                    {
                        foreach (var memberId in userToRemoveSetIds)
                        {
                            await _userService.ReassignToEmployeeRole(memberId);
                        }
                        var projectHistoryModel = userToRemove.Select(u =>
                        {
                            bool isPresent = u.AssignedUserId == incomingManangerId;
                            return new ProjectHistory
                            {
                                ProjectName = u.Project.Title,
                                ChangedByUser = "Admin",
                                ChangedByUserEmail = "Admin@gmail.com",
                                ChangedUser = u.AssignedUser.UserName,
                                ChangedUserEmail = u.AssignedUser.Email,
                                NewRole = isPresent ? "Project Manager" : "Employee",
                                PreviousRole = "Member",
                                ChangeType = isPresent ? Changed.Promoted : Changed.Removed,
                            };
                        }).ToList();
                        if (projectHistoryModel.Any())
                        {
                            var projectTitle = projectHistoryModel.Select(u => u.ProjectName).FirstOrDefault()
                                ?? throw new KeyNotFoundException("Project title not found");

                            //queue removed members mail notification
                            _projectEmailNotificationHelper.QueueRemovedMemberEmail(userToRemoveSetIds, projectTitle, null);

                            await _projectRepo.CreateProjectHistory(projectHistoryModel, null);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error! Failed to remove members from project");
                throw new InvalidOperationException($"Unexpected error! Member not removed form the project", ex);
            }

        }

        public bool ShouldSkipProjectUpdate(string incomingManagerId, string initialManagerId, HashSet<string> newMembers, bool hasChanged)
        {
            return incomingManagerId == initialManagerId
             && !newMembers.Any() && !hasChanged;
        }
    }
}
