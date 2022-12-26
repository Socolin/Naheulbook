using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Extensions.UnitOfWorks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
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
}