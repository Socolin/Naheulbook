namespace Naheulbook.Core.Features.Spell;

public class CharacterSpellBookNotFoundException(Guid spellBookId) : Exception
{
    public Guid SpellBookId { get; } = spellBookId;
}
