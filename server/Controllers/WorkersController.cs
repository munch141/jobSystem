using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Job>> Get()
        {
            return Ok(new Job(Guid.NewGuid(), "worker controller job", DateTime.Now));
        }
    }
}
