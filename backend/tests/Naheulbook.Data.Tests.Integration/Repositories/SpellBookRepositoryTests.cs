using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class SpellBookRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private SpellBookRepository _spellBookRepository;

    [SetUp]
    public void SetUp()
    {
        _spellBookRepository = new SpellBookRepository(RepositoryDbContext);
    }

    [Test]
    public async Task GetAllWithSpellsAsync_ReturnsBooksOrderedWithTheirSpellsOrdered()
    {
        TestDataUtil
            .AddSpellBook(out var book2, b => b.DisplayOrder = 2)
            .AddSpell(out var book2Spell, s => s.DisplayOrder = 1)
            .AddSpellBook(out var book1, b => b.DisplayOrder = 1)
            .AddSpell(out var book1Spell2, s => s.DisplayOrder = 2)
            .AddSpell(out var book1Spell1, s => s.DisplayOrder = 1)
            ;

        var actualBooks = await _spellBookRepository.GetAllWithSpellsAsync();

        AssertEntitiesAreLoadedWithSameOrder(actualBooks, [book1, book2]);
        AssertEntitiesAreLoadedWithSameOrder(actualBooks[0].Spells, [book1Spell1, book1Spell2]);
        AssertEntitiesAreLoadedWithSameOrder(actualBooks[1].Spells, [book2Spell]);
    }
}
