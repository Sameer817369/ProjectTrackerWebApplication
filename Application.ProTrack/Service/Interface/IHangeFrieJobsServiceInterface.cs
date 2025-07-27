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

        Task SendManagerAssignedNotificationAsync(HashSet<string> projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle);
        Task SendMembersAssignedNotificationAsync(HashSet<string> membersIds, string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle);
        Task SendManagerChangedNotificationToMemberAsync(HashSet<string> memberIds, string projectTitle, string projectManagerId, string? taskManagerId, string? taskTitle);
        Task SendRemovedNotificationToManagerAsync(HashSet<string> projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle, bool? isPreviousTaskManagerPresentInNewMembers);
        Task SendRemovedNotificationToMemberAsync(HashSet<string> memberIds, string projectTitle, string? taskTitle);
    }
}
