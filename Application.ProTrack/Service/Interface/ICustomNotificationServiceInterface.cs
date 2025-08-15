using Shared.ProTrack.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ProTrack.Service.Interface
{
    public interface ICustomNotificationServiceInterface
    {
       NotificationDto SendManagerAssignedNotificationAsync(HashSet<string> projectManagerId, string projectTitle, string? taskManagerId, string? taskTitle);
    }
}
