using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class TakeItemResponse
{
    public ItemResponse TakenItem { get; set; } = null!;
    public int RemainingQuantity { get; set; }
}