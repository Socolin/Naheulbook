using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
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
            TestDataUtil.AddOriginWithAllData();
            var origins = TestDataUtil.GetAll<Origin>();

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
    }
}