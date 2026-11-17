using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Job;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/jobs")]
[ApiController]
public class JobsController(IJobService jobService, IJobMapper jobMapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<JobResponse>>> GetAsync(CancellationToken cancellationToken)
    {
        var jobs = await jobService.GetJobsAsync(cancellationToken);

        return jobs.Select(jobMapper.ToResponse).ToList();
    }
}