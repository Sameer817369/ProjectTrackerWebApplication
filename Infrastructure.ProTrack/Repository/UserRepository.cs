using Domain.ProTrack.Models;
using Domain.ProTrack.RepoInterface;
using Infrastructure.ProTrack.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.ProTrack.Repository
{
    public class UserRepository : IUserRepoInterface
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _context;

        public UserRepository(UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor, ApplicationDbContext context)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _context = context;
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
                throw new ApplicationException($"Usexpected error while registering user {ex.Message}");
            }
        }
        public async Task DeleteUserAsync(AppUser user)
        {
            _context.Users.Remove(user);
        }

        public async Task<List<AppUser>> GetAllUserAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public Task<AppUser?> GetUserByIdAsync(string userId)
        {
            var user = _userManager.FindByIdAsync(userId);
            return user;
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
        public Task UpdateUserAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
