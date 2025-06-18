using Domain.ProTrack.Models;

namespace Domain.ProTrack.Interface
{
    public interface ITokenGeneratorServiceInterface
    {
        Task<string> CreateJwtTokenAsync(AppUser user);
        Task<string> CreateEmailConfirmationTokenAsync(AppUser user);
    }
}
