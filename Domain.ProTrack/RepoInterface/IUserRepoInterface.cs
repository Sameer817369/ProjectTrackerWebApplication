
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.ProTrack.RepoInterface
{
    public interface IUserRepoInterface
    {
        Task<IdentityResult> CreateUserAsync(AppUser userModel, string password);
        Task UpdateUserAsync(string userId);
        Task<List<AppUser>> GetAllUserAsync();
        Task DeleteUserAsync(AppUser user);
        Task<AppUser?> GetUserByIdAsync(string userId);
        Task<List<string>> GetUserRoleAsync(AppUser user);
        Task<string> GetCurrentUserId();
    }
}
