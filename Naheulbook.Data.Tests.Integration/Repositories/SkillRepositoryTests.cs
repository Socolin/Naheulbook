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
            var skills = await AddInDbAsync(CreateSkills());

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

        private static List<Skill> CreateSkills(int count = 3)
        {
            var skills = new List<Skill>();

            for (var i = 0; i < count; i++)
            {
                skills.Add(new SkillBuilder().WithDefaultTestInfo(i + 1).WithSkillEffect($"some-stat-{i + 1}").Build());
            }

            return skills;
        }
    }
}