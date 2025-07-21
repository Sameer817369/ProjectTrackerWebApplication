using Application.ProTrack.Service.Interface;
using Domain.ProTrack.Models;
using Microsoft.Extensions.Logging;

namespace Application.ProTrack.Service
{
    public class HangeFireJobService : IHangeFrieJobsServiceInterface
    {
        private readonly ICustomeEmailServiceInterface _customEmailService;
        private readonly ILogger<HangeFireJobService> _logger;

        public HangeFireJobService(ICustomeEmailServiceInterface customEmail, ILogger<HangeFireJobService> logger)
        {
            _customEmailService = customEmail;
            _logger = logger;
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

        public async Task SendProjectMemberAssignedEmailAsync(HashSet<string> memberIds, string managerId, string title)
        {
            try
            {
                await _customEmailService.SendProjectMembersAssignedEmailsAsync(memberIds, managerId, title);
                _logger.LogInformation("Project Assigned Email successfully send to the members of poject {titel}",title);
            }
            catch (Exception ex)
            {
                _logger.LogError("Project Assigned Email not send to the members of poject {titel}", title);
                throw new InvalidOperationException("Unexpected Error! Failed to send project assignment email to the members", ex);
            }
        }

        public async Task SendProjectManagerAssignedEmailAsync(string managerId, string title)
        {
            try
            {
                await _customEmailService.SendProjectManagerAssignedEmailsAsync(managerId, title);
                _logger.LogInformation("Project Assigned Email successfully send to the manager of poject {titel}", title);
            }
            catch (Exception ex)
            {
                _logger.LogError("Project Assigned Email not send to the manager of the {titel} project", title);
                throw new InvalidOperationException("Unexpected Error! Failed to send project assignment email to the manager", ex);
            }
        }
        public async Task SendRemovedFromProjectEmailToManagerAsync(string managerId, string title)
        {
            try
            {
                await _customEmailService.SendRemovedManagerFromProjectEmailsAsync(managerId, title);
                _logger.LogInformation("Manager updated Email successfully send to the previous manager of poject {titel}", title);
            }
            catch (Exception ex)
            {
                _logger.LogError("Manager updated  Email not send to the manager of the poject {titel}", title);
                throw new InvalidOperationException("Unexpected Error! Failed to send manager updated email to the updated manager", ex);
            }
        }
        public async Task SendRemovedFromProjectEmailToMemberAsync(HashSet<string>memberIds, string title)
        {
            try
            {
                await _customEmailService.SendRemovedMemberEmailsAsync(memberIds, title);
                _logger.LogInformation("Member removed Email successfully send to the removed members of poject {titel}", title);
            }
            catch (Exception ex)
            {
                _logger.LogError("Manager updated  Email not send to the members of the poject {titel}", title);
                throw new InvalidOperationException("Unexpected Error! Failed to send member updated email to the removed members", ex);
            }
        }
        public async Task SendManagerChangedEmailToMemberAsync(HashSet<string> memberIds, string title, string managerId)
        {
            try
            {
                await _customEmailService.SendManagerChangedEmailAsync(memberIds, title, managerId);
                _logger.LogInformation("Manager changed Email successfully send to the members of poject {titel}", title);
            }
            catch (Exception ex)
            {
                _logger.LogError("Manager changed Email not send to the members of the poject {titel}", title);
                throw new InvalidOperationException("Unexpected Error! Failed to send m anager changed email to the members", ex);
            }
        }
    }
}
