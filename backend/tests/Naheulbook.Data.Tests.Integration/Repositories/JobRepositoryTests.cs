using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

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
        TestDataUtil
            .AddStat()
            .AddSkill(out var skill1)
            .AddSkill(out var skill2)
            .AddJob(out var job)
            .AddJobBonus(out var jobBonus)
            .AddJobRequirement(out var jobRequirement)
            .AddJobRestriction(out var jobRestriction)
            .AddJobSkill(out var jobSkill1, skill1)
            .AddJobSkill(out var jobSkill2, skill2)
            .AddSpeciality(out var speciality)
            ;

        var actualJobs = await _jobRepository.GetAllWithAllDataAsync();

        AssertEntitiesAreLoaded(actualJobs, [job]);
        var actualJob = actualJobs.Single();
        AssertEntitiesAreLoaded(actualJob.Bonuses, [jobBonus]);
        AssertEntitiesAreLoaded(actualJob.Requirements, [jobRequirement]);
        AssertEntitiesAreLoaded(actualJob.Restrictions, [jobRestriction]);
        AssertEntitiesAreLoaded(actualJob.Skills, [jobSkill1, jobSkill2]);
        AssertEntityIsLoaded(actualJob.Skills.Single(x => x.SkillId == jobSkill1.SkillId), jobSkill1);
        AssertEntityIsLoaded(actualJob.Skills.Single(x => x.SkillId == jobSkill2.SkillId), jobSkill2);
        AssertEntitiesAreLoaded(actualJob.Specialities, [speciality]);
    }
}