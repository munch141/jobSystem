using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private JobQueue _jobQueue;

        public JobsController(JobQueue jobQueue)
        {
            _jobQueue = jobQueue;
        }

        [Route("pending")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetPendingJobs()
        {
            var pendingJobs = await _jobQueue.ListAsync();

            return Ok(pendingJobs);
        }
    }
}
