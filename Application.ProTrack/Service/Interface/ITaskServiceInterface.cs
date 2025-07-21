using Application.ProTrack.DTO.TaskDto;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using Shared.ProTrack.Dto;

namespace Application.ProTrack.Service
{
    public interface ITaskServiceInterface
    {
        Task<IdentityResult> CreateTask(CreateTaskDto createTask, Guid projectId);
        Task<IdentityResult> UpdateTaskAsync(UpdateTaskDto updateTask, Guid projectId, Guid taksId);
        Task<IdentityResult> RemoveTask(Guid taskId, Guid projectId);
        Task<GetTaskDetailsDto> GetTaskDetailsAsync(Guid projectId, Guid taskId);
    }

}
