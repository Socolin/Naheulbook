using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/jobs")]
[ApiController]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly IMapper _mapper;

    public JobsController(IJobService jobService, IMapper mapper)
    {
        _jobService = jobService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<JobResponse>>> GetAsync()
    {
        var jobs = await _jobService.GetJobsAsync();

        return _mapper.Map<List<JobResponse>>(jobs);
    }
}