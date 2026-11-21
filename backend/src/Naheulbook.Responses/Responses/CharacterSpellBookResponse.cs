using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

/// <summary>
/// A spell book given to a character on top of the ones of its job and specialities
/// </summary>
[PublicAPI]
public class CharacterSpellBookResponse
{
    public Guid SpellBookId { get; set; }
    public int? MaxLevel { get; set; }
}
