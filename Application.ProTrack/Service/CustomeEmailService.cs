using Application.ProTrack.Helper;
using Domain.ProTrack.Models;
using Domain.ProTrack.RepoInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Application.ProTrack.Service
{
    public class CustomeEmailService : ICustomeEmailServiceInterface
    {
        private readonly IEmailRepoInterface _emailRepo;
        private readonly ILogger<ICustomeEmailServiceInterface> _logger;
        private readonly IEmailServiceInterface _emailSender;
        private readonly UserManager<AppUser> _userManager;
        public CustomeEmailService(IEmailRepoInterface emailRepo, ILogger<ICustomeEmailServiceInterface> logger, IEmailServiceInterface emailSender, UserManager<AppUser> userManager)
        {
            _emailRepo = emailRepo;
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            try
            {

                var result = await _emailRepo.ConfirmEmailAsync(userId, token);
                if (result.Succeeded)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to confirm email of user {UserId}. Error: {ErrorMessage}", userId, ex.Message);
                throw new InvalidOperationException($"Unexpected Error! Failed to confirm email of user {userId}", ex);
            }
        }
        public async Task<(IdentityResult, string)> SendEmailConfirmation(AppUser user)
        {
            try
            {
                if (user == null) throw new UnauthorizedAccessException("User not found");
                var confirmationToken = await _emailRepo.GenerateEmailTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
                if (string.IsNullOrWhiteSpace(confirmationToken)) throw new InvalidOperationException("Token not created");
                var confirmationLink = $"http://localhost:5159/api/User/conform-email/{user.Id}/{encodedToken}";
                var body = $@"
                                    <h2>Hi {user.FirstName},</h2>
                                    <p>Thank you for registering. Please confirm your email by clicking the link below:</p>
                                    <a href='{confirmationLink}'>Confirm Email</a>
                                ";
                var emailMessage = EmailTempletHelper.WrapInStandardTemplate(user.UserName, body);

                var result = await _emailSender.CreateEmailAsync(user.Email, $"Confirm You Email {user.UserName}", emailMessage);
                if (!result.Succeeded)
                {
                    var error = "Failed";
                    return (IdentityResult.Failed([.. result.Errors]), error);
                }
                return (IdentityResult.Success, confirmationLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error! Failed to send email to {Username} of email address {Email}", user?.UserName ?? "Unknown", user?.Email ?? "Unknown");
                throw new InvalidOperationException($"Unexpected error! when sending conformation email to user {user.UserName}", ex);
            }
        }
        public async Task<IdentityResult> SendMembersAssignedEmailsAsync(HashSet<string> memberIds, string projectManagerId, string projectTitle, string? taskTitle, string? taskManagerId )
        {
            try
            {
                var projectManager = await _userManager.FindByIdAsync(projectManagerId);
                var taskManager = await _userManager.FindByIdAsync(taskManagerId);
                if (projectManager == null)
                    return IdentityResult.Failed(new IdentityError { Code = "ManagerNotFound", Description = "Manager not found" });

                // Prepare and send emails to members
                var emailTasks = memberIds.Select(async memberId =>
                {
                    var member = await _userManager.FindByIdAsync(memberId);
                    if (member == null)
                        throw new InvalidOperationException($"Member {memberId} not found.");

                    var body = taskManagerId == null ? $@"
                                                        <p>You have been added to the <strong>{projectTitle}</strong> project under manager <strong>{projectManager.UserName}</strong>.</p>
                                                        <p>Please visit your dashboard for details.</p>"
                                                     :
                                                     $@"
                                                        <p>You have been added to the <strong>{taskTitle}</strong> task in the project{projectTitle} under task manager <strong>{taskManager.UserName}</strong>.</p>
                                                        <p>Please visit your dashboard for details.</p>";



                    var subject = taskManager == null ? $"Assigned to project {projectTitle}"
                                        : $"Assigned to task {taskTitle}";
                                                        
                    var emailMessage = EmailTempletHelper.WrapInStandardTemplate(member.UserName, body);

                    var result = await _emailSender.CreateEmailAsync(member.Email, subject, emailMessage);
                    if (!result.Succeeded)
                        throw new InvalidOperationException($"Email not sent to member {member.UserName}.");
                });

                await Task.WhenAll(emailTasks);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending project assignment emails for project {projectTitle}", projectTitle);
                throw new InvalidOperationException("Unexpected error while sending project assignment emails", ex);
            }
        }
        public async Task<IdentityResult> SendManagerAssignedEmailsAsync(string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle)
        {
            try
            {
                var projectManager = await _userManager.FindByIdAsync(projectManagerId);
                var taskManager = await _userManager.FindByIdAsync(taskManagerId);
                if (projectManager == null)
                    return IdentityResult.Failed(new IdentityError { Code = "ManagerNotFound", Description = "Manager not found" });

                // Send email to manager

                var body = taskManagerId == null ? $@"
                                                     <p>You have been added to the <strong>{projectTitle} project</strong> as the <strong>project manager</strong>.</p>
                                                     <p>Please visit your dashboard for details.</p>"
                                                 :
                                                 $@"
                                                    <p>You have been added to the <strong>{taskTitle}</strong> task in the project {projectTitle} as the <strong>task manager</strong>.</p>
                                                    <p>Please visit your dashboard for details.</p>";

                var subject = taskManager == null ? $"Assigned to project {projectTitle}"
                                        : $"Assigned to task {taskTitle}";


                var name = taskManager == null ? projectManager.UserName
                                                : taskManager.UserName;

                var emailMessage = EmailTempletHelper.WrapInStandardTemplate(name, body);

                var email = taskManager == null ? projectManager.Email
                                                : taskManager.Email;

                var managerEmailResult = await _emailSender.CreateEmailAsync(email, subject, emailMessage);
                if (!managerEmailResult.Succeeded)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "ManagerEmailFailed",
                        Description = $"Email not sent to the manager {projectManager.UserName}"
                    });
                }
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending project manager assignment emails for project {projectTitle}", projectTitle);
                throw new InvalidOperationException("Unexpected error while sending project manager assignment emails", ex);
            }
        }

        public async Task<IdentityResult> SendRemovedManagerEmailsAsync(string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle,bool? isPreviousTaskManagerPresentInNewMembers)
        {
            try
            {
                var body = "";
                var projectManager = await _userManager.FindByIdAsync(projectManagerId);
                if (projectManager == null)
                    return IdentityResult.Failed(new IdentityError { Code = "ManagerNotFound", Description = "Manager not found" });
                var taskManager = await _userManager.FindByIdAsync(taskManagerId);

                var projectManagerRole = await _userManager.GetRolesAsync(projectManager);

                if (projectManager == null)
                    return IdentityResult.Failed(new IdentityError { Code = "ManagerNotFound", Description = "Manager not found" });

                // Send email to manager
                if(taskManager == null)
                {
                    body = projectManagerRole.Contains("Employee")
                           ? $@"<p>You have been removed from the <strong>{projectTitle} project</strong>.</p>
                                                <p>Please visit your dashboard for details.</p>"

                           : $@"<p>You have been demoted to <strong>member</strong> in the project <strong>{projectTitle}</strong>.</p>
                                                <p>Please visit your dashboard for details.</p>";
                }
                else
                {
                    body = isPreviousTaskManagerPresentInNewMembers == false
                            ? $@"<p>You have been removed from the <strong>{taskTitle} task in the project {projectTitle} by the project manager {projectManager.UserName}</strong>.</p>
                                                                    <p>Please visit your dashboard for details.</p>"

                            : $@"<p>You have been demoted to <strong>member</strong> in the task <strong>{taskTitle} of project {projectTitle} </strong>  by the project manager <strong> {projectManager.UserName} </strong>.</p>
                                                                    <p>Please visit your dashboard for details.</p>";
                }
  

                var subject = taskManager == null ? $"Manager Updated in the project {projectTitle}"
                        : $"Manager Updated in the task {taskTitle}";

                var email = taskManager == null ? projectManager.Email
                                                : taskManager.Email;

                var name = taskManager == null ? projectManager.UserName
                                               : taskManager.UserName;

                var emailMessage = EmailTempletHelper.WrapInStandardTemplate(name, body);

                var managerEmailResult = await _emailSender.CreateEmailAsync(email, subject, emailMessage);
                if (!managerEmailResult.Succeeded)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "ManagerEmailFailed",
                        Description = $"Email not sent to the manager {projectManager.UserName} or {taskManager.UserName}"
                    });
                }
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending removed from project emails for project {projectTitle} to the project manager or taskManager of the task {taskTitle}", projectTitle, taskTitle);
                throw new InvalidOperationException("Unexpected error while sending removed from project emails", ex);
            }
        }
        public async Task<IdentityResult> SendRemovedMemberEmailsAsync(HashSet<string> memberIds, string projectTitle, string? taskTitle)
        {
            try
            {
                var body = "";
                // Prepare and send emails to members
                foreach(var memberId in memberIds)
                {
                    var member = await _userManager.FindByIdAsync(memberId);
                    if (member == null)
                        throw new InvalidOperationException($"Member {memberId} not found.");
                    var memberRole = await _userManager.GetRolesAsync(member);
                    if(taskTitle == null)
                    {
                         body = memberRole.Contains("Project Manager")
                                   ? $@"<p>You have been promoted to <strong>project manager</strong> for the project <strong>{projectTitle}</strong>.</p>
                                    <p>Please visit your dashboard for details.</p>"

                                   : $@"<p>You have been removed from the project <strong>{projectTitle}</strong>.</p>
                                    <p>Please visit your dashboard for details.</p>";
                    }
                    else
                    {
                        body = memberRole.Contains("Task Manager")
                                  ? $@"<p>You have been promoted to <strong>task manager</strong> for the task <strong>{taskTitle}</strong> in the project <strong>{projectTitle}</strong>.</p>
                                                    <p>Please visit your dashboard for details.</p>"

                                  : $@"<p>You have been removed from the task<strong> {taskTitle}</strong> of the project <strong>{projectTitle}</strong>.</p>
                                                    <p>Please visit your dashboard for details.</p>";
                    }

                    var subject = taskTitle == null ? $"Member Role Updated in the project {projectTitle}"
                                                    : $"Member Role Updated in the task {taskTitle}";

                    var htmlMessage = EmailTempletHelper.WrapInStandardTemplate(member.UserName, body);

                    await _emailSender.CreateEmailAsync(member.Email, subject, htmlMessage);
                }
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending removed from project emails for project {projectTitle} to the members", projectTitle);
                throw new InvalidOperationException("Unexpected error while sending removed from project emails", ex);
            }
        }
        public async Task<IdentityResult> SendManagerChangedEmailAsync(HashSet<string>memberIds, string projectTitle, string? projectManagerId, string? taskTitle, string?taskManagerId)
        {
            try
            {
                var body = "";
                var projectManager = await _userManager.FindByIdAsync(projectManagerId);
                if (projectManager == null)
                    return IdentityResult.Failed(new IdentityError { Code = "ManagerNotFound", Description = "Manager not found" });

                var taskManager = await _userManager.FindByIdAsync(taskManagerId); 

                var emailTask = memberIds.Select(async memberId =>
                {
                    var member = await _userManager.FindByIdAsync(memberId);
                    if (member == null)
                        throw new KeyNotFoundException("members not found");
                    if(taskManagerId == null)
                    {
                        body = $@"
                                  <p>The current project manager for the project <strong>{projectTitle}</strong> is <strong>{projectManager.UserName}</strong>.</p>
                                  <p>Please visit your dashboard for details.</p>";

                    }
                    else
                    {
                        body = $@"
                                  <p>The current task manager for the task <strong>{taskTitle}</strong> of the project  <strong>{projectTitle}</strong> is <strong>{taskManager.UserName}</strong>.</p>
                                  <p>Please visit your dashboard for details.</p>";
                    }
                    var subject = taskManagerId == null ? $"Project Manager Changed in the project {projectTitle}"
                                                        :  $"Task Manager Changed in the task {taskTitle}";

                    var emailMessage = EmailTempletHelper.WrapInStandardTemplate(member.UserName, body);

                    var result = await _emailSender.CreateEmailAsync(member.Email, subject, emailMessage);
                    if (!result.Succeeded)
                        throw new InvalidOperationException($"Email not sent to member {member.UserName}.");
                });
                await Task.WhenAll(emailTask);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending manager changed email for project {projectTitle} to the members", projectTitle);
                throw new InvalidOperationException($"Unexpected error while sending manager changed email from the project {projectTitle}", ex);
            }

        }
    }
}
