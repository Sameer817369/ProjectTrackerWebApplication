using Application.ProTrack.DTO;

namespace Application.ProTrack.Service
{
    public interface IAuthServiceInterface
    {
        Task<string> LoginUserAsync(LoginUserDto loginUser);
    }
}
