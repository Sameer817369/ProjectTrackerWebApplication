using Shared.ProTrack.Dto;

namespace Application.ProTrack.Service.Interface
{
    public interface ICustomNotificationInterface
    {
        Task CreateNotificationToUsersAsync(NotificationDto notification, HashSet<string> userIds, string? taskManagerId);
    }
}
