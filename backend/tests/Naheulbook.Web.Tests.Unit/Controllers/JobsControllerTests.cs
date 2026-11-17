using AwesomeAssertions;
using NSubstitute;
using NUnit.Framework;
using Naheulbook.Core.Features.Job;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Controllers;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class JobsControllerTests
{
    private IJobService _jobService;
    private IJobMapper _jobMapper;
    private JobsController _jobsController;

    [SetUp]
    public void SetUp()
    {
        _jobService = Substitute.For<IJobService>();
        _jobMapper = Substitute.For<IJobMapper>();
        _jobsController = new JobsController(_jobService, _jobMapper);
    }

    [Test]
    public async Task CanGetJobs()
    {
        var entity1 = new JobEntity();
        var entity2 = new JobEntity();
        var jobs = new List<JobEntity> {entity1, entity2};
        var response1 = new JobResponse();
        var response2 = new JobResponse();

        _jobService.GetJobsAsync()
            .Returns(jobs);
        _jobMapper.ToResponse(entity1)
            .Returns(response1);
        _jobMapper.ToResponse(entity2)
            .Returns(response2);

        var result = await _jobsController.GetAsync();

        result.Value.Should().Equal(response1, response2);
    }
}