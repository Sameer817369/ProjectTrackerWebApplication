using Microsoft.AspNetCore.Identity;

namespace Application.ProTrack.Service
{
    public interface IEmailServiceInterface
    {
        public Task<IdentityResult> CreateEmailAsync(string email, string subject, string message);
    }
}
