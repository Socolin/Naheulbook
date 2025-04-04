using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Job;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/jobs")]
[ApiController]
public class JobsController(IJobService jobService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<JobResponse>>> GetAsync()
    {
        var jobs = await jobService.GetJobsAsync();

        return mapper.Map<List<JobResponse>>(jobs);
    }
}