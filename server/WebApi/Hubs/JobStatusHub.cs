using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs
{
    public class JobStatusHub : Hub<IJobStatusClient>
    {
        public async Task SendJobStatusUpdate(Guid jobId, string status)
            => await Clients.All.UpdateJobStatus(jobId, status);
    }
}