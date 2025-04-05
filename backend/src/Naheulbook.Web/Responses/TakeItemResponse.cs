using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class TakeItemResponse
{
    public ItemResponse TakenItem { get; set; } = null!;
    public int RemainingQuantity { get; set; }
}