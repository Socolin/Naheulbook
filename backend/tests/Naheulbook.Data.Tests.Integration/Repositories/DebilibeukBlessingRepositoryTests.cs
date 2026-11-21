using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class DebilibeukBlessingRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private DebilibeukBlessingRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _repository = new DebilibeukBlessingRepository(RepositoryDbContext);
    }

    [Test]
    public async Task GetAllOrderedByDiceRollAsync_ReturnsEntriesOrderedByDiceRoll()
    {
        TestDataUtil
            .AddDebilibeukBlessing(out var entry2, x => x.DiceRoll = 2)
            .AddDebilibeukBlessing(out var entry1, x => x.DiceRoll = 1)
            ;

        var actual = await _repository.GetAllOrderedByDiceRollAsync();

        AssertEntitiesAreLoadedWithSameOrder(actual, [entry1, entry2]);
    }
}
