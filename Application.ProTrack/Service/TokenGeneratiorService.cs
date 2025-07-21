using Domain.ProTrack.RepoInterface;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.ProTrack.Service
{
    public class TokenGeneratiorService : ITokenGeneratorServiceInterface
    {
        private readonly IUserRepoInterface _userRepo;
        private readonly ILogger<ITokenGeneratorServiceInterface> _logger;
        private readonly IEmailRepoInterface _emailRepo;
        public TokenGeneratiorService(IUserRepoInterface userRepo, ILogger<ITokenGeneratorServiceInterface> logger, IEmailRepoInterface emailRepo)
        {
            _userRepo = userRepo;
            _logger = logger;
            _emailRepo = emailRepo;
        }

        public async Task<string> CreateEmailConfirmationTokenAsync(AppUser user)
        {
            try
            {
                var token = await _emailRepo.GenerateEmailTokenAsync(user);
                if(string.IsNullOrEmpty(token)) throw new InvalidOperationException("Token not generated");
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                return encodedToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email token generation failed for user {UserId}, Email: {Email}", user?.Id ?? "Unknown", user?.Email ?? "Unknown");
                throw new InvalidOperationException($"Unexpected Error! Email Token Generation Failed {ex.Message}");
            }
        }
        public async Task<string> CreateJwtTokenAsync(AppUser user)
        {
            if (string.IsNullOrEmpty(user.Id)) throw new ArgumentNullException("User id is null or empty");
            var claims = new List<Claim> 
            { 
                 new(ClaimTypes.NameIdentifier, user.Id),
                 new(ClaimTypes.Email, user.Email),
                 new(ClaimTypes.Name, user.UserName)
            };
            var roles = await _userRepo.GetUserRoleAsync(user);
            if (roles.Any())
            {
                foreach (var role in roles)
                {
                    claims.Add(new(ClaimTypes.Role, role));
                }
            }
            else
            {
                claims.Add(new(ClaimTypes.Role, "Employee"));
            }
            var tokenKey = Environment.GetEnvironmentVariable("TOKEN");
            if (string.IsNullOrEmpty(tokenKey)) throw new InvalidOperationException("Token Key is Missing in Configuratuin");
            var issuer = Environment.GetEnvironmentVariable("ISSUER");
            var audience = Environment.GetEnvironmentVariable("AUDIENCE");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            if (string.IsNullOrEmpty(tokenKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                _logger.LogCritical("JWT environment variables are missing. TOKEN: {Token}, ISSUER: {Issuer}, AUDIENCE: {Audience}",tokenKey, issuer, audience);
                throw new InvalidOperationException("JWT environment variables not configured properly.");
            }
            var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha512);
            var descriptor = new JwtSecurityToken
                (
                    issuer: issuer,
                    audience: audience,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: credentials,
                    claims: claims
                );
            return new JwtSecurityTokenHandler().WriteToken(descriptor);
        }
    }
}
