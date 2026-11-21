namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class PostApplyEntropyEventRequest
{
    /// <summary>
    /// Id of the calamity (Mankdebol) or blessing (Débilibeuk) chosen by the GM
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Character designated by the D6 roll, when the event targets a single character
    /// </summary>
    public int? CharacterId { get; set; }
}
