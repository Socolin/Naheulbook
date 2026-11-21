using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Spell;

public interface ISpellMapper
{
    SpellResponse ToResponse(SpellEntity spell);
    SpellBookResponse ToResponse(SpellBookEntity spellBook);
    CharacterSpellBookResponse ToResponse(CharacterSpellBookEntity characterSpellBook);
}

public class SpellMapper : ISpellMapper
{
    public SpellResponse ToResponse(SpellEntity spell)
    {
        return new SpellResponse
        {
            Id = spell.Id,
            Name = spell.Name,
            Level = spell.Level,
            LevelLabel = spell.LevelLabel,
            Kind = spell.Kind,
            Category = spell.Category,
            Description = spell.Description,
            Data = MapperHelpers.FromJsonNotNull<SpellData>(spell.Data),
        };
    }

    public SpellBookResponse ToResponse(SpellBookEntity spellBook)
    {
        return new SpellBookResponse
        {
            Id = spellBook.Id,
            TechName = spellBook.TechName,
            Name = spellBook.Name,
            Kind = spellBook.Kind,
            JobId = spellBook.JobId,
            SpecialityId = spellBook.SpecialityId,
            GodId = spellBook.GodId,
            Data = MapperHelpers.FromJsonNotNull<SpellBookData>(spellBook.Data),
            Spells = spellBook.Spells
                .OrderBy(x => x.DisplayOrder)
                .Select(ToResponse)
                .ToList(),
        };
    }

    public CharacterSpellBookResponse ToResponse(CharacterSpellBookEntity characterSpellBook)
    {
        return new CharacterSpellBookResponse
        {
            SpellBookId = characterSpellBook.SpellBookId,
            MaxLevel = characterSpellBook.MaxLevel,
        };
    }
}
