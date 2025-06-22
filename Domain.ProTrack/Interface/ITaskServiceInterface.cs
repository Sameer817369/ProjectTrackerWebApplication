using Domain.ProTrack.DTO.TaskDto;
using Microsoft.AspNetCore.Identity;

namespace Domain.ProTrack.Interface
{
    public interface ITaskServiceInterface
    {
        Task<IdentityResult> CreateTask(CreateTaskDto createTask, Guid projectId);
        Task<IdentityResult> UpdateTaskAsync(UpdateTaskDto updateTask, Guid projectId, Guid taksId);
    }
}
