namespace Naheulbook.Shared.TransientModels;

[Serializable]
public class LapCountDecrement
{
    public string When { get; set; } = null!;
    public int FighterId { get; set; }
    public bool FighterIsMonster { get; set; }
}