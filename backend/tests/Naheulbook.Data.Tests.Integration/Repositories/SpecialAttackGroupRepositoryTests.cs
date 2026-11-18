using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class SpecialAttackGroupRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private SpecialAttackGroupRepository _specialAttackGroupRepository;

    [SetUp]
    public void SetUp()
    {
        _specialAttackGroupRepository = new SpecialAttackGroupRepository(RepositoryDbContext);
    }

    [Test]
    public async Task GetAllWithSpecialAttacksAsync_ReturnsGroupsOrderedWithTheirSpecialAttacksOrdered()
    {
        TestDataUtil
            .AddSpecialAttackGroup(out var group2, g => g.DisplayOrder = 2)
            .AddSpecialAttack(out var group2Attack, a => a.DisplayOrder = 1)
            .AddSpecialAttackGroup(out var group1, g => g.DisplayOrder = 1)
            .AddSpecialAttack(out var group1Attack2, a => a.DisplayOrder = 2)
            .AddSpecialAttack(out var group1Attack1, a => a.DisplayOrder = 1)
            ;

        var actualGroups = await _specialAttackGroupRepository.GetAllWithSpecialAttacksAsync();

        AssertEntitiesAreLoadedWithSameOrder(actualGroups, [group1, group2]);
        AssertEntitiesAreLoadedWithSameOrder(actualGroups[0].SpecialAttacks, [group1Attack1, group1Attack2]);
        AssertEntitiesAreLoadedWithSameOrder(actualGroups[1].SpecialAttacks, [group2Attack]);
    }
}
