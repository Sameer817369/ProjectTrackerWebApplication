using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.ProTrack.Service
{
    public interface ICustomeEmailServiceInterface
    {
        Task<(IdentityResult, string)> SendEmailConfirmation(AppUser user);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<IdentityResult> SendMembersAssignedEmailsAsync(HashSet<string> memberIds, string projectManagerId, string projectTitle, string? taskTitle, string? taskManagerId);
        Task<IdentityResult> SendManagerAssignedEmailsAsync(string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle);
        Task<IdentityResult> SendRemovedManagerEmailsAsync(string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle, bool? isPreviousTaskManagerPresentInNewMembers);
        Task<IdentityResult> SendRemovedMemberEmailsAsync(HashSet<string> memberIds, string projectTitle, string? taskTitle);
        Task<IdentityResult> SendManagerChangedEmailAsync(HashSet<string> memberIds, string projectTitle, string? projectManagerId, string? taskTitle, string? taskManagerId);
    }
}
