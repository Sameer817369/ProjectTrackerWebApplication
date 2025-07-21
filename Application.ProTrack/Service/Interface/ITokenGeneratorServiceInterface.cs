using Domain.ProTrack.Models;

namespace Application.ProTrack.Service
{
    public interface ITokenGeneratorServiceInterface
    {
        Task<string> CreateJwtTokenAsync(AppUser user);
        Task<string> CreateEmailConfirmationTokenAsync(AppUser user);
    }
}
