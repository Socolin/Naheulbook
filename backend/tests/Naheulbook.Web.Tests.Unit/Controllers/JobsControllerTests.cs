using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Features.Job;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class JobsControllerTests
{
    private IJobService _jobService;
    private IMapper _mapper;
    private JobsController _jobsController;

    [SetUp]
    public void SetUp()
    {
        _jobService = Substitute.For<IJobService>();
        _mapper = Substitute.For<IMapper>();
        _jobsController = new JobsController(_jobService, _mapper);
    }

    [Test]
    public async Task CanGetJobs()
    {
        var jobs = new List<JobEntity>();
        var expectedResponse = new List<JobResponse>();

        _jobService.GetJobsAsync()
            .Returns(jobs);
        _mapper.Map<List<JobResponse>>(jobs)
            .Returns(expectedResponse);

        var result = await _jobsController.GetAsync();

        result.Value.Should().BeSameAs(expectedResponse);
    }
}