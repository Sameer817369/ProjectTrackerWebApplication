using Domain.ProTrack.Models;

namespace Application.ProTrack.Service.Interface
{
    public interface IHangeFrieJobsServiceInterface
    {
        Task SendEmailConformationAsync(AppUser user);
        Task SendProjectMemberAssignedEmailAsync(HashSet<string>memberIds, string managerIds, string title);
        Task SendRemovedFromProjectEmailToMemberAsync(HashSet<string> memberIds, string title);
        Task SendRemovedFromProjectEmailToManagerAsync(string managerId, string title);
        Task SendManagerChangedEmailToMemberAsync(HashSet<string> memberIds, string title, string managerId);
        Task SendProjectManagerAssignedEmailAsync(string managerId, string title);
    }
}
