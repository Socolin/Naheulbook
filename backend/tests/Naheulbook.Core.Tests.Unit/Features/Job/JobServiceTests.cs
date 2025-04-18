using FluentAssertions;
using Naheulbook.Core.Features.Job;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Job;

public class JobServiceTests
{
    private IJobRepository _jobRepository;
    private JobService _jobService;

    [SetUp]
    public void SetUp()
    {
        var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWorkFactory.CreateUnitOfWork().Returns(unitOfWork);
        _jobRepository = Substitute.For<IJobRepository>();
        unitOfWork.Jobs.Returns(_jobRepository);
        _jobService = new JobService(unitOfWorkFactory);
    }

    [Test]
    public async Task CanGetJobs()
    {
        var expectedJobs = new List<JobEntity>();

        _jobRepository.GetAllWithAllDataAsync()
            .Returns(expectedJobs);

        var jobs = await _jobService.GetJobsAsync();

        jobs.Should().BeSameAs(expectedJobs);
    }
}