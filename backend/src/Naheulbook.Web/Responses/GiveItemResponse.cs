using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class GiveItemResponse
{
    public int RemainingQuantity { get; set; }
}