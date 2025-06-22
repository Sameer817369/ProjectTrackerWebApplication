using Domain.ProTrack.DTO;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.ProTrack.Interface.RepoInterface
{
    public interface IUserRepoInterface
    {
        Task<IdentityResult> CreateUserAsync(AppUser userModel, string password);
        Task UpdateUserAsync(string userId, UpdateUserDto updateUser);
        Task GetAllUserAsync();
        Task DeleteUserAsync(string userId);
        Task<AppUser?> GetUserByIdAsync(string userId);
        Task<List<string>> GetUserRoleAsync(AppUser user);
        Task<string> GetCurrentUserId();
    }
}
