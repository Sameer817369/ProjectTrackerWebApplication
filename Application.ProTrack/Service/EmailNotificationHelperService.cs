using Application.ProTrack.Service.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Application.ProTrack.Service
{
    public class EmailNotificationHelperService : IEmailNotificationHelperInterface
    {
        private readonly IEmailDispatcherServiceInterface _emailDispatcherService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<IEmailNotificationHelperInterface> _logger;
        public EmailNotificationHelperService(IEmailDispatcherServiceInterface emailDispatcherService, IBackgroundJobClient backgroundJobClient, ILogger<IEmailNotificationHelperInterface> logger)
        {
            _emailDispatcherService = emailDispatcherService;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }
        public void QueueProjectTaskCreationEmails(string projectManagerId, HashSet<string> members, string projectTitle, string? taskManagerId, string? taskTitle )
        {
            _emailDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for assigned manager in {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendManagerAssignedEmailAsync(projectManagerId, projectTitle, taskManagerId, taskTitle));
                return Task.CompletedTask;
            });

            _emailDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for assigned members in {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendMemberAssignedEmailAsync(members, projectManagerId, projectTitle, taskManagerId, taskTitle));
                return Task.CompletedTask;
            });
        }
        public void QueueManagerChangedEmail(HashSet<string> memebers, string projectTitle, string newProjectManagerId, string? newTaskManagerId, string? taskTitle)
        {
            _emailDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for manager updated in the {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendManagerChangedEmailToMemberAsync(memebers, projectTitle, newProjectManagerId, newTaskManagerId, taskTitle));
                return Task.CompletedTask;
            });
        }

        public void QueueManagerRemovedEmail(string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle, bool? isPreviousTaskManagerPresentInNewMembers)
        {
            _emailDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for removed manager in {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    job => job.SendRemovedEmailToManagerAsync(projectManagerId, projectTitle, taskManagerId, taskTitle, isPreviousTaskManagerPresentInNewMembers));
                return Task.CompletedTask;
            });
        }
        public void QueueNewlyAddedMembersEmail(HashSet<string> newMembers, string newProjectManagerId, string projectTitle, string? newTaskManagerId, string? taskTitle)
        {
            _emailDispatcherService.Queue(() => {
                _logger.LogInformation("Queueing email for newly added members in project {title}", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendMemberAssignedEmailAsync(newMembers,newProjectManagerId, projectTitle, newTaskManagerId, taskTitle));
                return Task.CompletedTask;
            });
        }
        public void QueueRemovedMemberEmail(HashSet<string> removedMemberIds, string projectTitle, string? taskTitle)
        {
            _emailDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for removed members in the {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    job => job.SendRemovedEmailToMemberAsync(removedMemberIds, projectTitle, taskTitle));
                return Task.CompletedTask;
            });
        }
    }
}
