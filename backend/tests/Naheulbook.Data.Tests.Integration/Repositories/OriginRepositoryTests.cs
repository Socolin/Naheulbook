using FluentAssertions;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

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
        var origins = TestDataUtil.GetAll<OriginEntity>();

        var actualOrigins = await _originRepository.GetAllWithAllDataAsync();

        actualOrigins.Should().BeEquivalentTo(
            origins,
            config => config
                .Excluding(o => o.Id)
                .Excluding(info => info.Path.EndsWith(".Stat"))
                .Excluding(info => info.Path.EndsWith(".Skill"))
                .IgnoringCyclicReferences()
        );
    }
}