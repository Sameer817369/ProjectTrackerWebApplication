using Domain.ProTrack.DTO;
using Domain.ProTrack.Interface.RepoInterface;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.ProTrack.Repository
{
    public class UserRepository : IUserRepoInterface
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserRepository(UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }
        public async Task<IdentityResult> CreateUserAsync(AppUser userModel, string password)
        {
            try
            {
                if (await _userManager.Users.AnyAsync(u => u.Email == userModel.Email || u.UserName == userModel.UserName)) throw new InvalidOperationException("User already exits");
                var result = await _userManager.CreateAsync(userModel, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userModel, "Employee");
                }
                return result;
            }
            catch(Exception ex)
            {
                throw new Exception($"Usexpected error while registering user {ex.Message}");
            }
        }
        public Task DeleteUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task GetAllUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AppUser?> GetUserByIdAsync(string userId)
        {
            var user = _userManager.FindByIdAsync(userId);
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("User not found");
            return user;
        }

        public Task UpdateUserAsync(string userId, UpdateUserDto updateUser)
        {
            throw new NotImplementedException();
        }

        public async Task<AppUser?> GetUserEmailAsync(string email)
        {
            try
            {
                var result = await _userManager.FindByEmailAsync(email) ?? throw new NullReferenceException("Error finding email");
                return result;
            }
            catch(Exception ex)
            {
                throw new KeyNotFoundException($"Unexpected Error! Email Not Found {ex.Message}");
            }
        }

        public async Task<List<string>> GetUserRoleAsync(AppUser user)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(user) ?? throw new NullReferenceException("Error finding roles");
                return (List<string>)roles;
            }
            catch(Exception ex)
            {
                throw new KeyNotFoundException($"Unexpected Error! Roles Not Found {ex.Message}");
            }
        }

        public Task<string> GetCurrentUserId()
        {
            try
            {
                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value ?? throw new UnauthorizedAccessException("User not found");
                return Task.FromResult(userId);
            }
            catch(Exception ex)
            {
                throw new KeyNotFoundException($"Unexpected Error! User not found {ex.Message}");
            }
        }
    }
}
