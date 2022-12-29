namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class PatchGroupConfigRequest
{
    public bool? AllowPlayersToSeeSkillGmDetails { get; set; }
    public bool? AllowPlayersToAddObject { get; set; }
    public bool? AllowPlayersToSeeGemPriceWhenIdentified { get; set; }
}