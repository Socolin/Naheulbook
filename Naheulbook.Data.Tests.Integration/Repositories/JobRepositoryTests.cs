using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class JobRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
        private JobRepository _jobRepository;

        [SetUp]
        public void SetUp()
        {
            _jobRepository = new JobRepository(RepositoryDbContext);
        }

        [Test]
        public async Task CanGetAllWithAllData()
        {
            TestDataUtil.AddJobWithAllData();

            var actualJobs = await _jobRepository.GetAllWithAllDataAsync();

            actualJobs.Should().BeEquivalentTo(
                TestDataUtil.GetAll<Job>(),
                config => config
                    .Excluding(o => o.Id)
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Stat"))
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Skill"))
                    .IgnoringCyclicReferences());
        }
    }
}