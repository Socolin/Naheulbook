using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.Tests.Integration.EntityBuilders;
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
            var jobs = await CreateJobsInDb();

            var actualJobs = await _jobRepository.GetAllWithAllDataAsync();

            actualJobs.Should().BeEquivalentTo(
                jobs,
                config => config
                    .Excluding(o => o.Id)
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Stat"))
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Skill"))
                    .IgnoringCyclicReferences());
        }

        private async Task<List<Job>> CreateJobsInDb()
        {
            var stat = new StatBuilder().WithDefaultTestInfo().Build();
            await AddInDbAsync(stat);

            var skill1 = new SkillBuilder().WithDefaultTestInfo().Build();
            var skill2 = new SkillBuilder().WithDefaultTestInfo().Build();
            await AddInDbAsync(skill1, skill2);

            var origin1 = new OriginBuilder().WithDefaultTestInfo().Build();
            var origin2 = new OriginBuilder().WithDefaultTestInfo().Build();
            await AddInDbAsync(origin1, origin2);

            var job = new JobBuilder()
                .WithDefaultTestInfo()
                .WithBonus()
                .WithRequirement(stat, null, 5)
                .WithRestriction()
                .WithDefaultSkill(skill1)
                .WithAvailableSkill(skill2)
                .WithOriginBlacklist(origin1)
                .WithOriginWhitelist(origin2)
                .Build();
            var jobs = await AddInDbAsync(job);

            var speciality = new SpecialityBuilder()
                .WithDefaultTestInfo()
                .WithJob(job)
                .Build();
            await AddInDbAsync(speciality);

            return jobs;
        }
    }
}