// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses;

public class TakeItemResponse
{
    public ItemResponse TakenItem { get; set; } = null!;
    public int RemainingQuantity { get; set; }
}