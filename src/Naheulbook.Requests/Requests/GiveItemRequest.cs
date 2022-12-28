namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class GiveItemRequest
{
    public int? Quantity { get; set; }
    public int CharacterId { get; set; }
}