using Domain.ProTrack.DTO.TaskDto;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.ProTrack.Interface.RepoInterface
{
    public interface ITaskRepositoryInterface
    {
        Task<IdentityResult> CreateTaskAsync(Tasks taskModel, List<ProjectUserTask> projectUserTasks);
        Task<IdentityResult> UpdateTaskAsync(Tasks taskModel, List<ProjectUserTask> projectUserTasks);
        Task<IdentityResult> DeleteTaskAsync();
        Task<string> GetProjectManagerAsync(Guid projectId);
        Task<List<string>> GetProjectMembersAsync(Guid projectId, List<string> memberUsername);
        //Task<string> GetProjectAsync(Guid projectId);
        Task<List<string>> GetProjectUserAsync(Guid projectId);
        Task<string> GetExistingTaskIdAsync(Guid taskId);
    }
}
