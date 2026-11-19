using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class HandicapRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private HandicapRepository _handicapRepository;

    [SetUp]
    public void SetUp()
    {
        _handicapRepository = new HandicapRepository(RepositoryDbContext);
    }

    [Test]
    public async Task GetAllOrderedByDiceRollAsync_ReturnsHandicapsOrderedByDiceRoll()
    {
        TestDataUtil
            .AddHandicap(out var handicap2, h => h.DiceRoll = 2)
            .AddHandicap(out var handicap1, h => h.DiceRoll = 1)
            ;

        var actualHandicaps = await _handicapRepository.GetAllOrderedByDiceRollAsync();

        AssertEntitiesAreLoadedWithSameOrder(actualHandicaps, [handicap1, handicap2]);
    }
}
