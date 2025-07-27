using Application.ProTrack.Service.Interface;
using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.ProTrack.Dto;

namespace Application.ProTrack.Service
{
    public class HangeFireJobService : IHangeFrieJobsServiceInterface
    {
        private readonly ICustomeEmailServiceInterface _customEmailService;
        private readonly ILogger<HangeFireJobService> _logger;
        private readonly ICustomNotificationInterface _customNotificationService;
        private readonly UserManager<AppUser> _userManager;


        public HangeFireJobService(ICustomeEmailServiceInterface customEmail, ILogger<HangeFireJobService> logger, ICustomNotificationInterface notificationDispatcher , UserManager<AppUser> userManager)
        {
            _customEmailService = customEmail;
            _logger = logger;
            _customNotificationService = notificationDispatcher;
            _userManager = userManager;
        }

        public async Task SendEmailConformationAsync(AppUser user)
        {
            try
            {
                await _customEmailService.SendEmailConfirmation(user);
                _logger.LogInformation("Confirmation email sent to user {User}", user.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError("Email confirmation sent to user {User} ({Email})", user.UserName, user.Email);
                throw new InvalidOperationException("Unexpected Error! Failed to send conformaiton email",ex);
            }
        }

        public async Task SendMemberAssignedEmailAsync(HashSet<string> memberIds, string projectManagerId, string projectTitle, string? taskManagetId, string? taskTitle)
        {
            try
            {
                await _customEmailService.SendMembersAssignedEmailsAsync(memberIds, projectManagerId, projectTitle, taskTitle, taskManagetId);
                _logger.LogInformation("Project Assigned Email successfully send to the members of poject {titel}",projectTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError("Project Assigned Email not send to the members of poject {titel}", projectTitle);
                throw new InvalidOperationException("Unexpected Error! Failed to send project assignment email to the members", ex);
            }
        }

        public async Task SendManagerAssignedEmailAsync(string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle )
        {
            try
            {
                await _customEmailService.SendManagerAssignedEmailsAsync(projectManagerId, projectTitle, taskManagerId, taskTitle);
                _logger.LogInformation("Project Assigned Email successfully send to the manager of poject {titel}", projectTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError("Project Assigned Email not send to the manager of the {titel} project", projectTitle);
                throw new InvalidOperationException("Unexpected Error! Failed to send project assignment email to the manager", ex);
            }
        }
        public async Task SendRemovedEmailToManagerAsync(string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle, bool? isPreviousTaskManagerPresentInNewMembers)
        {
            try
            {
                await _customEmailService.SendRemovedManagerEmailsAsync(projectManagerId, projectTitle, taskManagerId,taskTitle, isPreviousTaskManagerPresentInNewMembers);
                _logger.LogInformation("Manager updated Email successfully send to the previous manager of poject {titel}", projectTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError("Manager updated  Email not send to the manager of the poject {titel}", projectTitle);
                throw new InvalidOperationException("Unexpected Error! Failed to send manager updated email to the updated manager", ex);
            }
        }
        public async Task SendRemovedEmailToMemberAsync(HashSet<string>memberIds, string projectTitle, string? taskTitle)
        {
            try
            {
                await _customEmailService.SendRemovedMemberEmailsAsync(memberIds, projectTitle, taskTitle);
                _logger.LogInformation("Member removed Email successfully send to the removed members of poject {titel}", projectTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError("Manager updated  Email not send to the members of the poject {titel}", projectTitle);
                throw new InvalidOperationException("Unexpected Error! Failed to send member updated email to the removed members", ex);
            }
        }
        public async Task SendManagerChangedEmailToMemberAsync(HashSet<string> memberIds, string projectTitle, string projectManagerId, string? taskManagerId, string? taskTitle)
        {
            try
            {
                await _customEmailService.SendManagerChangedEmailAsync(memberIds, projectTitle, projectManagerId, taskTitle, taskManagerId);
                _logger.LogInformation("Manager changed Email successfully send to the members of poject {titel}", projectTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError("Manager changed Email not send to the members of the poject {titel}", projectTitle);
                throw new InvalidOperationException("Unexpected Error! Failed to send m anager changed email to the members", ex);
            }
        }
        public async Task SendManagerAssignedNotificationAsync(HashSet<string> projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle)
        {
            try
            {
                await _customNotificationService.CreateNotificationToUsersAsync(new NotificationDto
                {
                    Message = taskManagerId == null ? $"You have been assigned as project manager in the {projectTitle} project"
                                                    : $"You have been assigned as task manager in the {taskTitle} of the project {projectTitle}",
                    NotficationTitle = taskManagerId == null ? "Assigned to project"
                                                             : "Assigned to task"
                },projectManagerId, taskManagerId);
            
            }
            catch (Exception ex)
            {
                _logger.LogError("Project Assigned notification not send to the manager of the {titel} project", projectTitle);
                throw new InvalidOperationException("Unexpected Error! Failed to send project assignment notification to the manager", ex);
            }
        }

        public async Task SendMembersAssignedNotificationAsync(HashSet<string> membersIds, string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle)
        {
            try
            {
                var managers = await FindManagerInfo(projectManagerId, taskManagerId);
                await _customNotificationService.CreateNotificationToUsersAsync(new NotificationDto
                {
                    Message = taskTitle == null ? $"You have been assigned in the  project {projectTitle} under project manager {managers.projectManager.UserName}"
                                                    : $"You have been assigned in the task {taskTitle} of the project {projectTitle} under task manager {managers.taskManager.UserName}",
                    NotficationTitle = taskTitle == null ? "Assigned to project"
                                                             : "Assigned to task"
                }, membersIds, null);

            }
            catch (Exception ex)
            {
                _logger.LogError("Project Assigned notification not send to the members of the {titel} project", projectTitle);
                throw new InvalidOperationException("Unexpected Error! Failed to send project assignment notification to the manager", ex);
            }
        }
        public async Task SendManagerChangedNotificationToMemberAsync(HashSet<string> memberIds, string projectTitle, string projectManagerId, string? taskManagerId, string? taskTitle)
        {
            try
            {
                var managers = await FindManagerInfo(projectManagerId, taskManagerId);
                await _customNotificationService.CreateNotificationToUsersAsync(new NotificationDto
                {
                    Message = taskManagerId == null ? $"The current project manager for the project {projectTitle} is {managers.projectManager.UserName}"
                                            : $"The current task manager for the task {taskTitle} of the project {projectTitle} is <strong>{managers.taskManager.UserName}",
                    NotficationTitle = taskManagerId == null ? "Project manager updated"
                                                     : "Task manager updated"
                }, memberIds, taskManagerId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Manager changed notification not send to the members of the poject {titel}", projectTitle);
                throw new InvalidOperationException("Unexpected Error! Failed to send m anager changed email to the members", ex);
            }
        }

        public async Task SendRemovedNotificationToManagerAsync(HashSet<string> projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle, bool? isPreviousTaskManagerPresentInNewMembers)
        {
            try
            {
                var managers = await FindManagerInfo(projectManagerId.ToString(), taskManagerId);
                var projectManagerRole = await _userManager.GetRolesAsync(managers.projectManager);

                await _customNotificationService.CreateNotificationToUsersAsync(new NotificationDto
                {
                    Message = taskManagerId == null ? projectManagerRole.Contains("Employee")
                                                               ? $@"You have been removed from the <strong>{projectTitle} project."

                                                               : $@"You have been demoted to member in the project {projectTitle}." 
                           
                                                    : isPreviousTaskManagerPresentInNewMembers == false
                                                                ? $@"You have been removed from the {taskTitle} task in the project {projectTitle} by the project manager {managers.projectManager.UserName}."

                                                                : $@"You have been demoted to member in the task {taskTitle} of project {projectTitle}  by the project manager {managers.projectManager.UserName}",

                    NotficationTitle = taskManagerId == null ? 
                                $"Manager Updated in the project {projectTitle}"
                                : $"Manager Updated in the task {taskTitle}"

                },projectManagerId, taskManagerId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Manager updated  notification not send to the manager of the poject {titel}", projectTitle);
                throw new InvalidOperationException("Unexpected Error! Failed to send manager updated notification to the updated manager", ex);
            }
        }

        public async Task SendRemovedNotificationToMemberAsync(HashSet<string> memberIds, string projectTitle, string? taskTitle)
        {
            try
            {
                foreach (var memeberId in memberIds)
                {
                    var member = await _userManager.FindByIdAsync(memeberId);
                    var memebrRole = await _userManager.GetRolesAsync(member);

                    await _customNotificationService.CreateNotificationToUsersAsync(new NotificationDto
                    {
                        Message = taskTitle == null ? memebrRole.Contains("Project Manager")
                                      ? $@"You have been promoted to project manager for the project {projectTitle}"

                                      : $@"You have been removed from the project {projectTitle}"
                                : memebrRole.Contains("Task Manager")
                                      ? $@"<p>You have been promoted to <strong>task manager</strong> for the task <strong>{taskTitle}</strong> in the project <strong>{projectTitle}</strong>.</p>
                                                                        <p>Please visit your dashboard for details.</p>"

                                      : $@"You have been removed from the task {taskTitle} of the project {projectTitle}",

                        NotficationTitle = taskTitle == null ? $"Member Role Updated in the project {projectTitle}"

                                         : $"Member Role Updated in the task {taskTitle}"
                    }, memberIds, null);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Manager updated  Email not send to the members of the poject {titel}", projectTitle);
                throw new InvalidOperationException("Unexpected Error! Failed to send member updated email to the removed members", ex);
            }
        }



        public async Task<(AppUser projectManager,AppUser? taskManager)> FindManagerInfo(string projectManagerId, string? taskManagerId)
        {
            var projectManager = await  _userManager.FindByIdAsync(projectManagerId);
            var taskManager = await  _userManager.FindByIdAsync(taskManagerId);

            return (projectManager, taskManager);
        }
    }
}
