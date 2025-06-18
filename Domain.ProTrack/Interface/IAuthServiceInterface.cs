using Domain.ProTrack.DTO;

namespace Domain.ProTrack.Interface
{
    public interface IAuthServiceInterface
    {
        Task<string> LoginUserAsync(LoginUserDto loginUser);
    }
}
