using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateEffectSubCategoryRequest
{
    [StringLength(255, MinimumLength = 1)]
    public required string Name { get; set; }

    [Range(0, short.MaxValue)]
    public short DiceCount { get; set; }
    [Range(0, short.MaxValue)]
    public short DiceSize { get; set; }

    [StringLength(10_000)]
    public string? Note { get; set; }

    [Range(0, int.MaxValue)]
    public int TypeId { get; set; }
}