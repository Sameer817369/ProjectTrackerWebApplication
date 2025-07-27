using Shared.ProTrack.Dto;

namespace Shared.ProTrack.Interface
{
    public interface INotificationHub
    {
        Task ReceiveNotification(NotificationDto notification);
    }
}
