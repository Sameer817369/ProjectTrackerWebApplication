using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Application.ProTrack.Service
{
    public class EmailSenderService : IEmailServiceInterface
    {
        private readonly ILogger<IEmailServiceInterface> _logger;
        public EmailSenderService(ILogger<IEmailServiceInterface> logger)
        {
            _logger = logger;
        }
        public async Task<IdentityResult> CreateEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var emailSender = Environment.GetEnvironmentVariable("EMAIL");
                var passwordKey = Environment.GetEnvironmentVariable("PASSWORD");
                if (string.IsNullOrEmpty(emailSender) || string.IsNullOrEmpty(passwordKey)) throw new InvalidOperationException("Sender email or password is missing in configuration");
                using var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(emailSender, passwordKey)
                };
                var message = new MailMessage
                {
                    From = new MailAddress(emailSender, "ProTrack"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                message.To.Add(email);
                await client.SendMailAsync(message);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create email to {email}", email ?? "Unknown");
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "EmailSendFailure",
                    Description = "An unexpected error occurred while sending email."
                });
            }
        }
    }
}
