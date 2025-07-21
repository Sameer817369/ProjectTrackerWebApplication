using Application.ProTrack.Service.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Application.ProTrack.Service
{
    public class ProjectEmailNotificationHelperService : IProjectEmailNotificationHelperInterface
    {
        private readonly IEmailDispatcherServiceInterface _emailDispatcherService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<IProjectEmailNotificationHelperInterface> _logger;
        public ProjectEmailNotificationHelperService(IEmailDispatcherServiceInterface emailDispatcherService, IBackgroundJobClient backgroundJobClient, ILogger<IProjectEmailNotificationHelperInterface> logger)
        {
            _emailDispatcherService = emailDispatcherService;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }
        public void QueueProjectCreationEmails(string manager, HashSet<string> members, string title)
        {
            _emailDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for assigned manager in {title} project", title);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendProjectManagerAssignedEmailAsync(manager, title));
                return Task.CompletedTask;
            });

            _emailDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for assigned members in {title} project", title);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendProjectMemberAssignedEmailAsync(members, manager, title));
                return Task.CompletedTask;
            });
        }
        public void QueueManagerChangedEmail(HashSet<string> memebers, string projectTitle, string newManagerId)
        {
            _emailDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for manager updated in the {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendManagerChangedEmailToMemberAsync(memebers, projectTitle, newManagerId));
                return Task.CompletedTask;
            });
        }

        public void QueueManagerRemovedEmail(string managerId, string title)
        {
            _emailDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for removed manager in {title} project", title);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    job => job.SendRemovedFromProjectEmailToManagerAsync(managerId, title));
                return Task.CompletedTask;
            });
        }
        public void QueueNewlyAddedMembersEmail(HashSet<string> newMembers, string newManagerId, string projectTitle)
        {
            _emailDispatcherService.Queue(() => {
                _logger.LogInformation("Queueing email for newly added members in project {title}", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendProjectMemberAssignedEmailAsync(newMembers, newManagerId, projectTitle));
                return Task.CompletedTask;
            });
        }
        public void QueueRemovedMemberEmail(HashSet<string> removedMemberIds, string projectTitle)
        {
            _emailDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for removed members in the {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    job => job.SendRemovedFromProjectEmailToMemberAsync(removedMemberIds, projectTitle));
                return Task.CompletedTask;
            });
        }
    }
}
