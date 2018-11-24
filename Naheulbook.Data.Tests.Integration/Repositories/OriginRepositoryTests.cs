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
    public class OriginRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
        private OriginRepository _originRepository;

        [SetUp]
        public void SetUp()
        {
            _originRepository = new OriginRepository(RepositoryDbContext);
        }

        [Test]
        public async Task GetAllWithAllData()
        {
            var origins = await CreateOriginInDbAsync();

            var actualOrigins = await _originRepository.GetAllWithAllDataAsync();

            actualOrigins.Should().BeEquivalentTo(
                origins,
                config => config
                    .Excluding(o => o.Id)
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Stat"))
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Skill"))
                    .IgnoringCyclicReferences()
            );
        }

        private async Task<List<Origin>> CreateOriginInDbAsync()
        {
            var stat = new StatBuilder().WithDefaultTestInfo().Build();
            await AddInDbAsync(stat);

            var skill1 = new SkillBuilder().WithDefaultTestInfo(1).Build();
            var skill2 = new SkillBuilder().WithDefaultTestInfo(2).Build();
            await AddInDbAsync(skill1, skill2);

            var origin = new OriginBuilder()
                .WithDefaultTestInfo()
                .WithInfo()
                .WithBonus()
                .WithRequirement(stat)
                .WithDefaultSkill(skill1)
                .WithAvailableSkill(skill2)
                .Build();

            return await AddInDbAsync(origin);
        }
    }
}