namespace Naheulbook.Core.Features.Spell;

public class SpellBookNotFoundException(Guid spellBookId) : Exception
{
    public Guid SpellBookId { get; } = spellBookId;
}
