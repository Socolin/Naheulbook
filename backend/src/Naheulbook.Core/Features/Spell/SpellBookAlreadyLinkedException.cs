namespace Naheulbook.Core.Features.Spell;

public class SpellBookAlreadyLinkedException(Guid spellBookId) : Exception
{
    public Guid SpellBookId { get; } = spellBookId;
}
