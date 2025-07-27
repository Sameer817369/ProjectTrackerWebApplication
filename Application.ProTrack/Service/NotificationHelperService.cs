using Application.ProTrack.Service.Interface;
using Hangfire;
using Microsoft.Extensions.Logging;
using Shared.ProTrack.Dto;

namespace Application.ProTrack.Service
{
    public class NotificationHelperService : INotificationHelperInterface
    {
        private readonly INotificationDispatcherServiceInterface _notificationDispatcherService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<INotificationHelperInterface> _logger;
        public NotificationHelperService(INotificationDispatcherServiceInterface notificationDispatcherService, 
            IBackgroundJobClient backgroundJobClient, 
            ILogger<INotificationHelperInterface> logger)
        {
            _notificationDispatcherService = notificationDispatcherService;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }
        public void QueueProjectTaskCreationEmails(string projectManagerId, HashSet<string> members, string projectTitle, string? taskManagerId, string? taskTitle )
        {
            _notificationDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for assigned manager in {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendManagerAssignedEmailAsync(projectManagerId, projectTitle, taskManagerId, taskTitle));
                return Task.CompletedTask;
            });

            _notificationDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for assigned members in {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendMemberAssignedEmailAsync(members, projectManagerId, projectTitle, taskManagerId, taskTitle));
                return Task.CompletedTask;
            });

            _notificationDispatcherService.Queue(() =>
            {
            _logger.LogInformation("Queueing notification for assigned manager in {title} project", projectTitle);
            _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                jobs => jobs.SendManagerAssignedNotificationAsync(new HashSet<string> { projectManagerId }, projectTitle, taskManagerId, taskTitle));
                return Task.CompletedTask;
            });

            _notificationDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing notification for assigned manager in {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendMembersAssignedNotificationAsync(members, projectManagerId, projectTitle, taskManagerId, taskTitle));
                return Task.CompletedTask;
            });
        }
        public void QueueManagerChangedEmail(HashSet<string> memebers, string projectTitle, string newProjectManagerId, string? newTaskManagerId, string? taskTitle)
        {
            _notificationDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for manager updated in the {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendManagerChangedEmailToMemberAsync(memebers, projectTitle, newProjectManagerId, newTaskManagerId, taskTitle));
                return Task.CompletedTask;
            });

            _notificationDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for manager updated in the {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendManagerChangedNotificationToMemberAsync(memebers, projectTitle, newProjectManagerId, newTaskManagerId, taskTitle));
                return Task.CompletedTask;
            });
        }

        public void QueueManagerRemovedEmail(string projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle, bool? isPreviousTaskManagerPresentInNewMembers)
        {
            _notificationDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for removed manager in {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    job => job.SendRemovedEmailToManagerAsync(projectManagerId, projectTitle, taskManagerId, taskTitle, isPreviousTaskManagerPresentInNewMembers));
                return Task.CompletedTask;
            });

            _notificationDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing notification for removed manager in {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    job => job.SendRemovedNotificationToManagerAsync(new HashSet<string> { projectManagerId }, projectTitle, taskManagerId, taskTitle, isPreviousTaskManagerPresentInNewMembers));
                return Task.CompletedTask;
            });
        }
        public void QueueNewlyAddedMembersEmail(HashSet<string> newMembers, string newProjectManagerId, string projectTitle, string? newTaskManagerId, string? taskTitle)
        {
            _notificationDispatcherService.Queue(() => {
                _logger.LogInformation("Queueing email for newly added members in project {title}", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendMemberAssignedEmailAsync(newMembers,newProjectManagerId, projectTitle, newTaskManagerId, taskTitle));
                return Task.CompletedTask;
            });

            _notificationDispatcherService.Queue(() => {
                _logger.LogInformation("Queueing notification for newly added members in project {title}", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    jobs => jobs.SendMemberAssignedEmailAsync(newMembers, newProjectManagerId, projectTitle, newTaskManagerId, taskTitle));
                return Task.CompletedTask;
            });
        }
        public void QueueRemovedMemberEmail(HashSet<string> removedMemberIds, string projectTitle, string? taskTitle)
        {   
            _notificationDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing email for removed members in the {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    job => job.SendRemovedEmailToMemberAsync(removedMemberIds, projectTitle, taskTitle));
                return Task.CompletedTask;
            });

            _notificationDispatcherService.Queue(() =>
            {
                _logger.LogInformation("Queueing notification for removed members in the {title} project", projectTitle);
                _backgroundJobClient.Enqueue<IHangeFrieJobsServiceInterface>(
                    job => job.SendRemovedNotificationToMemberAsync(removedMemberIds, projectTitle, taskTitle));
                return Task.CompletedTask;
            });
        }
    }
}
