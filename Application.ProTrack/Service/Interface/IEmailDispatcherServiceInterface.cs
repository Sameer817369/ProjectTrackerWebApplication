using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ProTrack.Service.Interface
{
    public interface IEmailDispatcherServiceInterface
    {
        void Queue(Func<Task> emailJobs);
        Task DispatchAsync();
    }
}
