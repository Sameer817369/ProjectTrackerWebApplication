using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.ProTrack.Service
{
    public interface ICustomeEmailServiceInterface
    {
        Task<(IdentityResult, string)> SendEmailConfirmation(AppUser user);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<IdentityResult> SendProjectMembersAssignedEmailsAsync(HashSet<string> memberIds, string managerId, string projectTitle);
        Task<IdentityResult> SendRemovedManagerFromProjectEmailsAsync(string managerId, string projectTitle);
        Task<IdentityResult> SendRemovedMemberEmailsAsync(HashSet<string> memberIds, string projectTitle);
        Task<IdentityResult> SendManagerChangedEmailAsync(HashSet<string> memberIds, string projectTitle, string managerId);
        Task<IdentityResult> SendProjectManagerAssignedEmailsAsync(string managerId, string projectTitle);
    }
}
