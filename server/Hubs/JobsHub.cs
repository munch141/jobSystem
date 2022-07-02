using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs
{
    public class JobsHub : Hub<IJobsClient>
    {
        public async Task SendJobStatusUpdate(Job job, string status)
            => await Clients.All.UpdateJobStatus(job, status);

        public async Task EnqueueJob(Job job)
            => await Clients.All.EnqueueJob(job);

         public async Task DequeueJob(Guid jobId)
            => await Clients.All.DequeueJob(jobId);
    }
}