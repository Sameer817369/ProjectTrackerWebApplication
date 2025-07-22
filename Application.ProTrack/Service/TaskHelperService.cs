using Application.ProTrack.DTO.TaskDto;
using Application.ProTrack.Service.Interface;
using Domain.ProTrack.Models;
using Domain.ProTrack.RepoInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using static Domain.ProTrack.Enum.Enum;

namespace Application.ProTrack.Service
{
    public class TaskHelperService : ITaskHelperServiceInterface
    {
        private readonly IProjectRepoInterface _projectRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITaskRepositoryInterface _taskRepo;
        private readonly ILogger<ITaskHelperServiceInterface> _logger;
        private readonly IEmailNotificationHelperInterface _emailNotificationHelper;
        public TaskHelperService(IProjectRepoInterface projectRepo,
            UserManager<AppUser> userManager, 
            ITaskRepositoryInterface taskRepo,
            ILogger<ITaskHelperServiceInterface> logger,
            IEmailNotificationHelperInterface emailNotificationHelper)
        {
            _projectRepo = projectRepo;
            _userManager = userManager;
            _taskRepo = taskRepo;
            _logger = logger;
            _emailNotificationHelper = emailNotificationHelper;
        }
        public bool HasChanged(Tasks existingTask, UpdateTaskDto updateTask)
        {
            return existingTask.Title != updateTask.Title
               || existingTask.EndDate != updateTask.EndDate
               || existingTask.StartDate != updateTask.StartDate
               || existingTask.Description != updateTask.Description
               || existingTask.Priority != updateTask.Priority;
        }

        public async Task<bool> RemoveMemberFromTask(HashSet<string> incomingMemberId, Guid taskId, AppUser initialManager, AppUser incomingManager, Guid projectId)
        {
            try
            {
                var membersToRemove = incomingMemberId.ToList();
                var projectManagerId = await _projectRepo.GetProjectManagerAsync(projectId);
                var projectManager = _userManager.FindByIdAsync(projectManagerId);
                var trackedMembersToRemove = await _taskRepo.GetMembersToRemove(membersToRemove, taskId);
                var incomingManagerId = incomingManager.Id;
                var result = await _taskRepo.DeleteMemberFromTask(trackedMembersToRemove.ToList());
                if (result)
                {
                    var taskHistoryModel = new List<TaskHistory>();
                    taskHistoryModel.AddRange(
                    trackedMembersToRemove.Select(u =>
                    {
                        var isPromoted = u.ProjectUser.AssignedUserId == incomingManagerId;
                        return new TaskHistory
                        {
                            ProjectName = u.ProjectUser.Project.Title,
                            TaskName = u.Task.Title,
                            ChangedUser = u.ProjectUser.AssignedUser.UserName,
                            ChangedUserEmail = u.ProjectUser.AssignedUser.Email,
                            ChangedByUser = isPromoted ? projectManager.Result.UserName : initialManager.UserName,
                            ChangedByUserEmail = isPromoted ? projectManager.Result.Email : initialManager.Email,
                            PreviousRole = "Member",
                            ChangeType = isPromoted ? Changed.Promoted : Changed.Removed,
                            NewRole = isPromoted ? "Task Manager" : null
                        };
                    }).ToList());
                    if (taskHistoryModel.Any())
                    {
                        await _taskRepo.CreateTaskHistoryForMembers(taskHistoryModel);
                        var name = taskHistoryModel.Select(u => (u.TaskName, u.ProjectName)).FirstOrDefault();
                        _emailNotificationHelper.QueueRemovedMemberEmail(incomingMemberId, name.ProjectName, name.TaskName);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error! Failed to remove members from task '{taskId}'", taskId);
                throw new InvalidOperationException($"Unexpected Error! Failed to remove members from task", ex);
            }
        }

        public async Task<bool> RemovePerviousManager(Guid taskId, string initialTaskManagerId, Guid projectId, HashSet<string> assignedUserId)
        {
            try
            {
                var project = await _projectRepo.GetProjectAsync(projectId);
                var task = project.Tasks.FirstOrDefault(u => u.TaskId == taskId);
                var managerToRemove = await _taskRepo.GetTaskManagerToRemove(taskId, initialTaskManagerId);
                var projectManagerId = await _projectRepo.GetProjectManagerAsync(projectId);
                var projectManager = _userManager.FindByIdAsync(projectManagerId);
                var managerDetails = managerToRemove.ProjectUser.AssignedUser;
                var isPresent = assignedUserId.Contains(managerDetails.Id);
                var removed = await _taskRepo.DeleteManagerFromTask(managerToRemove);
                if (removed)
                {
                    var user = await _userManager.FindByIdAsync(initialTaskManagerId);
                    await _userManager.RemoveFromRoleAsync(user, "Task Manager");
                    var taskHistoryModel = new TaskHistory
                    {
                        ProjectName = project.Title,
                        TaskName = task.Title,
                        ChangedUser = managerDetails.UserName,
                        ChangedUserEmail = managerDetails.Email,
                        ChangedByUser = projectManager.Result.UserName,
                        ChangedByUserEmail = projectManager.Result.Email,
                        PreviousRole = "Task Manager",
                        ChangeType = isPresent ? Changed.Demoted : Changed.Removed,
                        NewRole = isPresent ? "Member" : null
                    };
                    if (taskHistoryModel != null)
                    {
                        await _taskRepo.CreateTaskHistoryForManagers(taskHistoryModel);
                        _emailNotificationHelper.QueueManagerRemovedEmail(projectManagerId, taskHistoryModel.ProjectName, initialTaskManagerId, task.Title, isPresent);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error! Failed to remove previous manager '{managerId}' from task '{taskId}'", taskId, initialTaskManagerId);
                throw new InvalidOperationException($"Unexpected Error! Failed to remove previous manager from task", ex);
            }
        }

        public bool ShouldSkipTaskUpdate(string incomingManagerId, string initialManagerId, bool hasChanged, HashSet<string> newMembersIds)
        {
            return incomingManagerId == initialManagerId
                && !hasChanged
                && !newMembersIds.Any();
        }
    }
}
