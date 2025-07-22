using Domain.ProTrack.Models;

namespace Application.ProTrack.Service.Interface
{
    public interface IHangeFrieJobsServiceInterface
    {
        Task SendEmailConformationAsync(AppUser user);
        Task SendMemberAssignedEmailAsync(HashSet<string> memberIds, string projectManagerId, string projectTitle, string? taskManagetId, string? taskTitle);
        Task SendManagerAssignedEmailAsync(string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle);
        Task SendRemovedEmailToManagerAsync(string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle, bool? isPreviousTaskManagerPresentInNewMembers);
        Task SendRemovedEmailToMemberAsync(HashSet<string> memberIds, string projectTitle, string? taskTitle);
        Task SendManagerChangedEmailToMemberAsync(HashSet<string> memberIds, string projectTitle, string projectManagerId, string? taskManagerId, string? taskTitle);
    }
}
