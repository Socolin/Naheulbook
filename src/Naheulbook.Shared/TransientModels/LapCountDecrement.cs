// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Shared.TransientModels;

public class LapCountDecrement
{
    public string When { get; set; } = null!;
    public int FighterId { get; set; }
    public bool FighterIsMonster { get; set; }
}