namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateEffectSubCategoryRequest
{
    public required string Name { get; set; }
    public short DiceCount { get; set; }
    public short DiceSize { get; set; }
    public string? Note { get; set; }
    public int TypeId { get; set; }
}