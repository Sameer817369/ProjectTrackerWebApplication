using Domain.ProTrack.DTO;
using Domain.ProTrack.Interface;
using Domain.ProTrack.Interface.RepoInterface;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.ProTrack.Service
{
    public class UserService : IUserServiceInterface
    {
        private readonly IUserRepoInterface _userRepo;
        private readonly ILogger<IUserServiceInterface> _logger;
        private readonly ICustomeEmailServiceInterface _customeEmail;
        public UserService(IUserRepoInterface userRepo, ILogger<IUserServiceInterface> logger, ICustomeEmailServiceInterface customeEmail)
        {
            _userRepo = userRepo;
            _logger = logger;
            _customeEmail = customeEmail;
        }

        public async Task<(IdentityResult,string)> CreateUserAsync(RegisterUserDto registerUser)
        {
            try
            {
                var userModel = new AppUser
                {
                    Email = registerUser.EmialAddress,
                    UserName = registerUser.UserName,
                    FirstName = registerUser.FirstName,
                    LastName = registerUser.LastName,
                    City = registerUser.City,
                    Street = registerUser.Street,
                    Age = registerUser.Age,
                    PhoneNumber = registerUser.ContactNumber,
                    PhoneNumberConfirmed = true
                };
                var result = await _userRepo.CreateUserAsync(userModel, registerUser.Password);
                if (!result.Succeeded)
                {
                    var error = "Failed to register";
                    return (IdentityResult.Failed([..result.Errors]),error);
                    throw new ApplicationException("User not registered");
                }
                var confirmationLink = await _customeEmail.SendEmailConfirmation(userModel);
                return confirmationLink;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected error! Failed to register user {Username} having email {Email}", registerUser?.UserName ?? "Unknown", registerUser?.EmialAddress ?? "Unknown");
                throw new InvalidOperationException($"Failure to register user {ex.Message}");
            }
        }
        public Task DeleteUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task GetAllUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task GetUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }
        public Task UpdateUserAsync(string userId, UpdateUserDto updateUser)
        {
            throw new NotImplementedException();
        }
    }
}
