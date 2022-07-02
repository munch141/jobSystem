using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly JobQueue _jobQueue;

        private readonly IHubContext<JobsHub, IJobsClient> _jobsHubContext;

        public JobsController(
            JobQueue jobQueue,
            IHubContext<JobsHub, IJobsClient> jobsHubContext)
        {
            _jobQueue = jobQueue;
            _jobsHubContext = jobsHubContext;
        }

        [Route("pending")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetPendingJobs()
        {
            var pendingJobs = await _jobQueue.ListAsync();

            return Ok(pendingJobs);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<CreatedResult> CreateJob(Job job)
        {
            await _jobQueue.EnqueueAsync(job);

            await _jobsHubContext.Clients.All.EnqueueJob(job);

            return Created($"job/{job.Id}", job);
        }
    }
}
