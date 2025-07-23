using Application.ProTrack.DTO;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.ProTrack.Service
{
    public interface IUserServiceInterface
    {
        Task<(IdentityResult, string)> CreateUserAsync(RegisterUserDto registerUser);
        Task UpdateUserAsync(string userId, UpdateUserDto updateUser);
        Task<List<AppUser>> GetAllUserAsync();
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<AppUser> GetUserByIdAsync(string userId);
        Task<bool> AssignRoleToEmployee(string managerId, HashSet<string> membersIds);
        Task<bool> ReassignToEmployeeRole(string assignedUserId);
        Task<AppUser> GetCurrentUser();
        Task<bool> ReassignToMemberRole(string assignedUserId);
    }
}
