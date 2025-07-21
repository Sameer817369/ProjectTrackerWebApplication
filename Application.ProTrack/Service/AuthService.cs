using Domain.ProTrack.RepoInterface;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Application.ProTrack.DTO;

namespace Application.ProTrack.Service
{
    public class AuthService : IAuthServiceInterface
    {
        private readonly IUserRepoInterface _userRepo;
        private readonly ITokenGeneratorServiceInterface _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<IAuthServiceInterface> _logger;
        private readonly IEmailRepoInterface _emailRepo;
 
        public AuthService(IUserRepoInterface userRepo, ITokenGeneratorServiceInterface tokenService, SignInManager<AppUser> signInManager, ILogger<IAuthServiceInterface> logger, IEmailRepoInterface emailRepo)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _logger = logger;
            _emailRepo = emailRepo;
        }
        public async Task<string> LoginUserAsync(LoginUserDto loginUser)
        {
            try
            {
                var user = await _emailRepo.GetUserEmailAsync(loginUser.Email) ?? throw new UnauthorizedAccessException("Invalid email or password");
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginUser.Password, false);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Login falied");
                }
                var token = await _tokenService.CreateJwtTokenAsync(user);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for user with email: {Email}", loginUser?.Email ?? "Unknown");
                throw new InvalidOperationException($"Unexpected Error! User Not Logged In {ex}");
            }
        }
    }
}
