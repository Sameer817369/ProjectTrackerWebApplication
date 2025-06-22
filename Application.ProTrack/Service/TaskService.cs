using Domain.ProTrack.DTO.TaskDto;
using Domain.ProTrack.Interface;
using Domain.ProTrack.Interface.RepoInterface;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using static Domain.ProTrack.Enum.Enum;

namespace Application.ProTrack.Service
{
    public class TaskService : ITaskServiceInterface
    {
        private readonly ITaskRepositoryInterface _taskRepo;
        private readonly ILogger<ITaskServiceInterface> _logger;
        private readonly IUserRepoInterface _userRepo;
        public TaskService(ITaskRepositoryInterface taskRepo, ILogger<ITaskServiceInterface> logger, IUserRepoInterface userRepo)
        {
            _taskRepo = taskRepo;
            _logger = logger;
            _userRepo = userRepo;
        }
        public async Task<IdentityResult> CreateTask(CreateTaskDto createTask, Guid projectId)
        {
            try
            {
                var currentUserId = await _userRepo.GetCurrentUserId();
                var managerId = await _taskRepo.GetProjectManagerAsync(projectId) ?? throw new InvalidOperationException("Couldnt fetch manager");
                var membersIds = await _taskRepo.GetProjectMembersAsync(projectId, createTask.MemberUsername) ?? throw new InvalidOperationException("Couldnt fetch members");
                //var project = await _taskRepo.GetProjectAsync(projectId);
                if(currentUserId.ToString()!= managerId)
                {
                    throw new UnauthorizedAccessException("You cannot assign task to the project as your are not part of it");
                }
                else
                {
                    var taskModel = new Tasks
                    {
                        ProjectId = projectId,
                        Title = createTask.Title,
                        Description = createTask.Description,
                        StartDate = createTask.StartDate,
                        EndDate = createTask.EndDate,
                        Status = Status.Pending,
                    };
                    var projectTaskUserModel = new List<ProjectUserTask>();
                    foreach (var memberId in membersIds)
                    {
                        projectTaskUserModel.Add(new ProjectUserTask
                        {
                            ProjectUserId = Guid.Parse(memberId),
                            TaskId = taskModel.TaskId,
                        });
                    }
                    var result = await _taskRepo.CreateTaskAsync(taskModel, projectTaskUserModel);
                    if (result.Succeeded)
                    {
                        return IdentityResult.Success;
                    }
                    return IdentityResult.Failed(new IdentityError { Description = "Failed to create task" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected Error! Failed to create taks for project {projectId}");
                throw new InvalidOperationException($"Unexpected Error! Failed to create task {ex.Message}");
            }
        }

        public async Task<IdentityResult> UpdateTaskAsync(UpdateTaskDto updateTask, Guid projectId, Guid taskId)
        {
            try
            {
                var currentUserId = await _userRepo.GetCurrentUserId();
                var managerId = await _taskRepo.GetProjectManagerAsync(projectId);
                var memberId = await _taskRepo.GetProjectMembersAsync(projectId, updateTask.MemberUsername);
                var taks = await _taskRepo.GetExistingTaskIdAsync(taskId);
                var existingMemberId = await _taskRepo.GetProjectUserAsync(projectId);
                if (currentUserId != managerId)
                {
                    throw new UnauthorizedAccessException("You cannot assign task to the project as your are not part of it");
                }
                if (!existingMemberId.Any(id=> memberId.Contains(id)) && taks == taskId.ToString())
                {
                    var taskmodel = new Tasks
                    {
                        TaskId = taskId,
                        Title = updateTask.Title,
                        Description = updateTask.Description,
                        StartDate = updateTask.StartDate,
                        EndDate = updateTask.EndDate,
                    };
                    var projectUserTaskModle = new List<ProjectUserTask>();
                    foreach (var memberIds in memberId)
                    {
                        projectUserTaskModle.Add(new ProjectUserTask
                        {
                            ProjectUserId = Guid.Parse(memberIds)
                        });
                    }
                    var result = await _taskRepo.UpdateTaskAsync(taskmodel, projectUserTaskModle);
                    if (result.Succeeded)
                    {
                        return IdentityResult.Success;
                    }
                    return IdentityResult.Failed(new IdentityError { Description = "Unexpected Error! Cannot Add Exisiting Member To Task Again"});
                }
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected Error! Failed to update taks for project {projectId}");
                throw new InvalidOperationException($"Unexpected Error! Failed to update task {ex.Message}");
            }
        }
    }
}
