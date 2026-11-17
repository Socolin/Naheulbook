using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class GiveItemResponse
{
    public int RemainingQuantity { get; set; }
}