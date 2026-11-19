using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class TalentGroupRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private TalentGroupRepository _talentGroupRepository;

    [SetUp]
    public void SetUp()
    {
        _talentGroupRepository = new TalentGroupRepository(RepositoryDbContext);
    }

    [Test]
    public async Task GetAllWithTalentsAsync_ReturnsGroupsOrderedWithTheirTalentsOrdered()
    {
        TestDataUtil
            .AddTalentGroup(out var group2, g => g.DisplayOrder = 2)
            .AddTalent(out var group2Talent, t => t.DisplayOrder = 1)
            .AddTalentGroup(out var group1, g => g.DisplayOrder = 1)
            .AddTalent(out var group1Talent2, t => t.DisplayOrder = 2)
            .AddTalent(out var group1Talent1, t => t.DisplayOrder = 1)
            ;

        var actualGroups = await _talentGroupRepository.GetAllWithTalentsAsync();

        AssertEntitiesAreLoadedWithSameOrder(actualGroups, [group1, group2]);
        AssertEntitiesAreLoadedWithSameOrder(actualGroups[0].Talents, [group1Talent1, group1Talent2]);
        AssertEntitiesAreLoadedWithSameOrder(actualGroups[1].Talents, [group2Talent]);
    }
}
