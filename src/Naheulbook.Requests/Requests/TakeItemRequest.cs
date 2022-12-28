namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class TakeItemRequest
{
    public int? Quantity { get; set; }
    public int CharacterId { get; set; }
}