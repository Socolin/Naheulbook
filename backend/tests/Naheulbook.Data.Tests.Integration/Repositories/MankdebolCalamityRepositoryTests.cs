using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class MankdebolCalamityRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private MankdebolCalamityRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _repository = new MankdebolCalamityRepository(RepositoryDbContext);
    }

    [Test]
    public async Task GetAllOrderedByDiceRollAsync_ReturnsEntriesOrderedByDiceRoll()
    {
        TestDataUtil
            .AddMankdebolCalamity(out var entry2, x => x.DiceRoll = 2)
            .AddMankdebolCalamity(out var entry1, x => x.DiceRoll = 1)
            ;

        var actual = await _repository.GetAllOrderedByDiceRollAsync();

        AssertEntitiesAreLoadedWithSameOrder(actual, [entry1, entry2]);
    }
}
