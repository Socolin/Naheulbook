using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class SkillRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
        private SkillRepository _skillRepository;

        [SetUp]
        public void SetUp()
        {
            _skillRepository = new SkillRepository(RepositoryDbContext);
        }

        [Test]
        public async Task CanGetAllWithEffects()
        {
            var skills = TestDataUtil
                .AddSkill()
                .AddSkill()
                .GetAll<Skill>();

            var actualSkills = await _skillRepository.GetAllWithEffectsAsync();

            actualSkills.Should().BeEquivalentTo(
                skills,
                config => config
                    .Excluding(s => s.Id)
                    .Excluding(s => s.JobSkills)
                    .Excluding(s => s.OriginSkills)
                    .Excluding(info => info.SelectedMemberPath == "Skill")
                    .IgnoringCyclicReferences()
            );
        }
    }
}