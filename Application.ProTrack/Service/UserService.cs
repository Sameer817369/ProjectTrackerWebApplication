using Application.ProTrack.DTO;
using Application.ProTrack.Service.Interface;
using Domain.ProTrack.Models;
using Domain.ProTrack.RepoInterface;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.ProTrack.Service
{
    public class UserService : IUserServiceInterface
    {
        private readonly IUserRepoInterface _userRepo;
        private readonly ILogger<IUserServiceInterface> _logger;
        private readonly ICustomeEmailServiceInterface _customeEmail;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        public UserService(
            IUserRepoInterface userRepo,
            ILogger<IUserServiceInterface> logger,
            ICustomeEmailServiceInterface customeEmail,
            UserManager<AppUser> userManager,
            IHttpContextAccessor contextAccessor, 
            IBackgroundJobClient backgroundJobClient)
        {
            _userRepo = userRepo;
            _logger = logger;
            _customeEmail = customeEmail;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _backgroundJobClient = backgroundJobClient;
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
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(jobs => jobs.SendEmailConformationAsync(userModel));
                var confirmationLink = await _customeEmail.SendEmailConfirmation(userModel);
                return confirmationLink;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected error! Failed to register user {Username} having email {Email}", registerUser?.UserName ?? "Unknown", registerUser?.EmialAddress ?? "Unknown");
                throw new ApplicationException($"Failure to register user {ex.Message}");
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
        public async Task<bool> AssignRoleToEmployee(string managerId, HashSet<string> membersIds)
        {
            try
            {
                var manager = await _userManager.FindByIdAsync(managerId) ?? throw new KeyNotFoundException("Manager not found");
                var existingManagerRole = await _userManager.GetRolesAsync(manager);
                if (existingManagerRole.Any())
                {
                    await _userManager.RemoveFromRolesAsync(manager, existingManagerRole);
                }
                await _userManager.AddToRoleAsync(manager, "Project Manager");
                
                foreach (var memberId in membersIds)
                {
                    if(memberId == managerId)
                    {
                        continue;
                    }
                    var member = await _userManager.FindByIdAsync(memberId) ?? throw new KeyNotFoundException("Manager not found");
                    var existingMemberRole = await _userManager.GetRolesAsync(member);
                    if (!existingMemberRole.Contains("Member"))
                    {
                        await _userManager.RemoveFromRolesAsync(member, existingMemberRole);
                        await _userManager.AddToRoleAsync(member, "Member");
                    }   
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected Error! Failed to reassign role back to employee");
                throw new InvalidOperationException($"Unexpected Error! Failed to assign role to employees {ex.Message}");
            }
        }
        public async Task<bool> ReassignToEmployeeRole(string assignedUserId)
        {
            try
            {
                var users = await _userManager.FindByIdAsync(assignedUserId)
                  ?? throw new KeyNotFoundException("Members not found");
                var existingRole = await _userManager.GetRolesAsync(users);
                if (existingRole.Any())
                {
                    await _userManager.RemoveFromRolesAsync(users, existingRole);
                }
                await _userManager.AddToRoleAsync(users, "Employee");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected Error! Failed to reassign role back to employee for userId: '{assignedUserId}'");
                throw new InvalidOperationException($"Unexpected Error! Failed to reassign role to employees {ex.Message}");
            }
        }
        public async Task<AppUser> GetCurrentUser()
        {
            try
            {
                var currentUserId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if(currentUserId != null)
                {
                    var currentUser = await _userManager.FindByIdAsync(currentUserId);
                    return currentUser;
                }
                throw new KeyNotFoundException("Current User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected Error! Failed to get current user details");
                throw new InvalidOperationException($"Unexpected Error!  Failed to get current user details {ex.Message}");
            }
        }
    }
}
