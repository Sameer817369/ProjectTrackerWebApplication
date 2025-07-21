using Application.ProTrack.Service.Interface;

namespace Application.ProTrack.Service
{
    public class EmailDispatcherService : IEmailDispatcherServiceInterface
    {
        private readonly List<Func<Task>> _emailQueue = new();
        public async Task DispatchAsync()
        {
            foreach (var job in _emailQueue)
            {
                await job();
            }
            _emailQueue.Clear();
        }
        public void Queue(Func<Task> emailJobs)
        {
            _emailQueue.Add(emailJobs);
        }
    }
}
