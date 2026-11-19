using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class MasteryGroupRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private MasteryGroupRepository _masteryGroupRepository;

    [SetUp]
    public void SetUp()
    {
        _masteryGroupRepository = new MasteryGroupRepository(RepositoryDbContext);
    }

    [Test]
    public async Task GetAllWithMasteriesAsync_ReturnsGroupsOrderedWithTheirMasteriesOrdered()
    {
        TestDataUtil
            .AddMasteryGroup(out var group2, g => g.DisplayOrder = 2)
            .AddMastery(out var group2Mastery, t => t.DisplayOrder = 1)
            .AddMasteryGroup(out var group1, g => g.DisplayOrder = 1)
            .AddMastery(out var group1Mastery2, t => t.DisplayOrder = 2)
            .AddMastery(out var group1Mastery1, t => t.DisplayOrder = 1)
            ;

        var actualGroups = await _masteryGroupRepository.GetAllWithMasteriesAsync();

        AssertEntitiesAreLoadedWithSameOrder(actualGroups, [group1, group2]);
        AssertEntitiesAreLoadedWithSameOrder(actualGroups[0].Masteries, [group1Mastery1, group1Mastery2]);
        AssertEntitiesAreLoadedWithSameOrder(actualGroups[1].Masteries, [group2Mastery]);
    }
}
