using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Matchers;
using System.ComponentModel.DataAnnotations;

namespace WebApiJobs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IScheduler jobs;

        public JobsController(IScheduler jobs)
        {
            this.jobs = jobs;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> All(CancellationToken ct)
        {
            var r = await jobs.GetJobKeys(GroupMatcher<JobKey>.AnyGroup(), ct);

            var infos = r.Select(jobKey => jobs.GetJobDetail(jobKey, ct).Result)
                .Where(x => x != null)
                .ToDictionary(
                    x => x!.Key.Name,
                    x => x!.JobDataMap.ToDictionary()
                );

            return Ok(infos);
        }

        [HttpGet("{jobName:alpha}/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Info([Required] string jobName, CancellationToken ct)
        {
            var jobKey = new JobKey(jobName);
            IJobDetail? job = await jobs.GetJobDetail(jobKey, ct);
            if (job == null)
            {
                return NotFound();
            }

            return Ok(job.JobDataMap.ToDictionary());
        }
    }
}
