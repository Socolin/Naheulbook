namespace Naheulbook.Shared.TransientModels;

[Serializable]
public class GroupConfig
{
    public bool AllowPlayersToSeeSkillGmDetails { get; set; }
    public bool AllowPlayersToAddObject { get; set; } = true;
    public bool AllowPlayersToSeeGemPriceWhenIdentified { get; set; }
    public bool AutoIncrementMonsterNumber { get; set; } = true;
    public bool AutoIncrementMonsterColor { get; set; } = true;
}