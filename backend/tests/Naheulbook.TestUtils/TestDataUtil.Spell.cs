using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddSpellBook(Action<SpellBookEntity> customizer = null)
    {
        return AddSpellBook(out _, customizer);
    }

    public TestDataUtil AddSpellBook(out SpellBookEntity spellBook, Action<SpellBookEntity> customizer = null)
    {
        spellBook = new SpellBookEntity
        {
            Id = Guid.NewGuid(),
            TechName = RngUtil.GetRandomString("some-spell-book-tech-name"),
            Name = RngUtil.GetRandomString("some-spell-book-name"),
            Kind = "mage",
            DisplayOrder = Random.Shared.Next(1, 100),
            Data = """{"description": "some-description", "categories": ["some-category"]}""",
            Spells = [],
        };

        return SaveEntity(spellBook, customizer);
    }

    public TestDataUtil AddSpell(Action<SpellEntity> customizer = null)
    {
        return AddSpell(out _, customizer);
    }

    public TestDataUtil AddSpell(out SpellEntity spell, Action<SpellEntity> customizer = null)
    {
        var spellBook = GetLast<SpellBookEntity>();
        spell = new SpellEntity
        {
            Id = Guid.NewGuid(),
            SpellBookId = spellBook.Id,
            SpellBook = spellBook,
            Name = RngUtil.GetRandomString("some-spell-name"),
            Level = 3,
            LevelLabel = "Niveau 3",
            Kind = "spell",
            Category = "some-category",
            DisplayOrder = Random.Shared.Next(1, 100),
            Description = RngUtil.GetRandomString("some-spell-description"),
            Data = """{"usages": ["some-usage"], "cost": {"text": "4 PA", "kind": "fixed", "astralPoints": 4}}""",
        };

        return SaveEntity(spell, customizer);
    }

    public TestDataUtil AddCharacterSpellBook(Action<CharacterSpellBookEntity> customizer = null)
    {
        return AddCharacterSpellBook(out _, customizer);
    }

    public TestDataUtil AddCharacterSpellBook(out CharacterSpellBookEntity characterSpellBook, Action<CharacterSpellBookEntity> customizer = null)
    {
        var character = GetLast<CharacterEntity>();
        var spellBook = GetLast<SpellBookEntity>();
        characterSpellBook = new CharacterSpellBookEntity
        {
            CharacterId = character.Id,
            SpellBookId = spellBook.Id,
            MaxLevel = 2,
        };

        return SaveEntity(characterSpellBook, customizer);
    }
}
