namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class AddCharacterSpellBookRequest
{
    public Guid SpellBookId { get; set; }

    public int? MaxLevel { get; set; }
}
