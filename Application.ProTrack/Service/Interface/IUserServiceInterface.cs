using Application.ProTrack.DTO;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.ProTrack.Service
{
    public interface IUserServiceInterface
    {
        Task<(IdentityResult, string)> CreateUserAsync(RegisterUserDto registerUser);
        Task UpdateUserAsync(string userId, UpdateUserDto updateUser);
        Task GetAllUserAsync();
        Task DeleteUserAsync(string userId);
        Task GetUserByIdAsync(string userId);
        Task<bool> AssignRoleToEmployee(string managerId, HashSet<string> membersIds);
        Task<bool> ReassignToEmployeeRole(string assignedUserId);
        Task<AppUser> GetCurrentUser();
    }
}
