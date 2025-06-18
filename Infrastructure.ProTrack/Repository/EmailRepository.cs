using Domain.ProTrack.Interface.RepoInterface;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Infrastructure.ProTrack.Repository
{
    public class EmailRepository : IEmailRepoInterface
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<IEmailRepoInterface> _logger;
        public EmailRepository(UserManager<AppUser> userManager, ILogger<IEmailRepoInterface> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId) ?? throw new UnauthorizedAccessException("User not found");
                var decodeToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token)) ?? throw new InvalidOperationException("Failed to decode token");
                var result = await _userManager.ConfirmEmailAsync(user, decodeToken);
                if (!result.Succeeded)
                {
                    return IdentityResult.Failed([.. result.Errors]);
                }
                return IdentityResult.Success;
            }
            catch(Exception ex)
            {
                throw new Exception("Unexpected error! Failed to confirm error",ex);
            }
        }

        public async Task<string> GenerateEmailTokenAsync(AppUser user)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                if (string.IsNullOrWhiteSpace(token)) throw new InvalidOperationException("Email conformation token not generated");
                return token;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error! Email Confirmation Token Generation Failed For User {Username}", user?.UserName ?? "Unknown");
                throw new Exception("Unexpected Error! Faild to Generate Conformation Token");
            }
        }

        public async Task<AppUser?> GetUserEmailAsync(string email)
        {
            try
            {
                var userEmail =await _userManager.FindByEmailAsync(email);
                if (userEmail == null) throw new NullReferenceException("Email not found");
                return userEmail;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "User email not found");
                throw new KeyNotFoundException("Unexpected Error! Email not found");
            }
        }
    }
}
