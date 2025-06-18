using Domain.ProTrack.DTO;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.ProTrack.Interface
{
    public interface IUserServiceInterface
    {
        Task<(IdentityResult, string)> CreateUserAsync(RegisterUserDto registerUser);
        Task UpdateUserAsync(string userId, UpdateUserDto updateUser);
        Task GetAllUserAsync();
        Task DeleteUserAsync(string userId);
        Task GetUserByIdAsync(string userId);
    }
}
