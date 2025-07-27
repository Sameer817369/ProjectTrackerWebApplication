using Application.ProTrack.Service.Interface;
using Microsoft.AspNetCore.SignalR;
using Shared.ProTrack.Dto;
using Shared.ProTrack.Interface;

namespace ProTrack.Hubs
{
    public class CustomNotificationSerivce : ICustomNotificationInterface
    {
        private readonly IHubContext<NotificationsHub, INotificationHub> _hubContext;
        private readonly ILogger<ICustomNotificationInterface> _logger;
        public CustomNotificationSerivce(IHubContext<NotificationsHub, INotificationHub> hubContext, ILogger<ICustomNotificationInterface> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }
        public async Task CreateNotificationToUsersAsync(NotificationDto notification, HashSet<string> userIds, string? taskManagerId)
        {
            try
            {
                foreach (var userId in userIds)
                {
                    await _hubContext.Clients.User(userId).ReceiveNotification(notification);
                    _logger.LogInformation($"Notification successfully send to the user {userId}");
                }
                _logger.LogInformation("Notification successfully send to all the users");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Falied to send notification to the users {userId}", userIds);
                throw new ApplicationException($"Internal Server Error! Failed to send notifications to the users {ex.Message}");
            }

        }
    }
}
