using Application.ProTrack.Service.Interface;
using Shared.ProTrack.Dto;

namespace Application.ProTrack.Service
{
    public class CustomNotificationService : ICustomNotificationServiceInterface
    {

        public NotificationDto SendManagerAssignedNotificationAsync(HashSet<string> projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle)
        {
            try
            {
                var message = new NotificationDto
                {
                    Message = taskManagerId == null ? $"You have been assigned as project manager in the {projectTitle} project"
                                                    : $"You have been assigned as task manager in the {taskTitle} of the project {projectTitle}",
                    NotficationTitle = taskManagerId == null ? "Assigned to project"
                                                             : "Assigned to task"
                };
                return message;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unexpected Error! Failed to send project assignment notification to the manager", ex);
            }
        }
    }
}
