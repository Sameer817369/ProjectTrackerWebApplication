using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using Shared.ProTrack.Dto;

namespace Domain.ProTrack.RepoInterface
{
    public interface ITaskRepositoryInterface
    {
        Task<bool> CreateTaskAsync(Tasks taskModel, List<ProjectUserTask> projectUserTasks);
        Task<bool> UpdateTaskAsync(Tasks taskModel, List<ProjectUserTask> projectUserTasks);
        Task<bool> DeleteTaskAsync(Tasks taskToDelete,Guid taskId);
        Task<Tasks> GetExistingTaskAsync(Guid taskId, Guid projectId);
        Task<HashSet<string>> GetProjectUserTaskMembers(Guid taksId);
        Task<bool> DeleteTaskAsync(Tasks taskToDelete);
        Task<bool> DeleteMemberFromTask(List<ProjectUserTask> toRemove);
        Task<bool> DeleteManagerFromTask(ProjectUserTask toRemove);
        Task<HashSet<ProjectUserTask>> GetMembersToRemove(List<string> idsToRemove, Guid taskId);
        Task<bool> CreateTaskHistoryForMembers(List<TaskHistory> taskHistoryModel);
        Task<bool> CreateTaskHistoryForManagers(TaskHistory taskHistoryModel);
        Task<GetTaskDetailsDto> GetTaskDetails(Guid projectId, Guid taskId);
        Task<string> GetProjectUserTaskManager(Guid taskId, Guid projectId);
        Task<ProjectUserTask> GetTaskManagerToRemove(Guid taskId, string taskManagerId);


    }
}
