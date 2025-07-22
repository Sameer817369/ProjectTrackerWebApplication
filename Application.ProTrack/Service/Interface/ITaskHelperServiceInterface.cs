using Application.ProTrack.DTO.TaskDto;
using Domain.ProTrack.Models;

namespace Application.ProTrack.Service.Interface
{
    public interface ITaskHelperServiceInterface
    {
        bool HasChanged(Tasks existingTask, UpdateTaskDto updateTask);
        bool ShouldSkipTaskUpdate(string incomingManagerId, string initialManagerId, bool hasChanged, HashSet<string> newMembersIds);
        Task<bool> RemoveMemberFromTask(HashSet<string> incomingMemberId, Guid taskId, AppUser initialManager, AppUser incomingManager, Guid projectId);
        Task<bool> RemovePerviousManager(Guid taskId, string initialTaskManagerId, Guid projectId, HashSet<string> AssignedUserId);
    }
}
