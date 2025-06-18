using Domain.ProTrack.Interface;
using Domain.ProTrack.Interface.RepoInterface;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Application.ProTrack.Service
{
    public class CustomeEmailService : ICustomeEmailServiceInterface
    {
        private readonly IEmailRepoInterface _emailRepo;
        private readonly ILogger<ICustomeEmailServiceInterface> _logger;
        private readonly IEmailServiceInterface _emailSender;
        public CustomeEmailService(IEmailRepoInterface emailRepo, ILogger<ICustomeEmailServiceInterface> logger, IEmailServiceInterface emailSender)
        {
            _emailRepo = emailRepo;
            _logger = logger;
            _emailSender = emailSender;
        }
        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            try
            {

                var result = await _emailRepo.ConfirmEmailAsync(userId, token);
                if (result.Succeeded)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to confirm email of user {UserId}. Error: {ErrorMessage}", userId, ex.Message);
                throw new InvalidOperationException($"Unexpected Error! Failed to confirm email of user {userId}", ex);
            }
        }
        public async Task<(IdentityResult, string)> SendEmailConfirmation(AppUser user)
        {
            try
            {
                if (user == null) throw new UnauthorizedAccessException("User not found");
                var confirmationToken = await _emailRepo.GenerateEmailTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
                if (string.IsNullOrWhiteSpace(confirmationToken)) throw new InvalidOperationException("Token not created");
                var confirmationLink = $"http://localhost:5159/api/User/confirm-email?userId={user.Id}&token={encodedToken}";
                var emailMessage = $@"
                                    <h2>Hi {user.FirstName},</h2>
                                    <p>Thank you for registering. Please confirm your email by clicking the link below:</p>
                                    <a href='{confirmationLink}'>Confirm Email</a>
                                ";
                var result = await _emailSender.CreateEmailAsync(user.Email, "Email Conformation", emailMessage);
                if (!result.Succeeded)
                {
                    var error = "Failed";
                    return (IdentityResult.Failed([.. result.Errors]), error);
                }
                return (IdentityResult.Success, confirmationLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error! Failed to send email to {Username} of email address {Email}", user?.UserName ?? "Unknown", user?.Email ?? "Unknown");
                throw new InvalidOperationException($"Unexpected error! when sending conformation email to user {user.UserName}", ex);
            }

        }
    }
}
