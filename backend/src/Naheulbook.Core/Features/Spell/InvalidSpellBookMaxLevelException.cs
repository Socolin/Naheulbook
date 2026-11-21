namespace Naheulbook.Core.Features.Spell;

public class InvalidSpellBookMaxLevelException(int maxLevel) : Exception($"Invalid max level: {maxLevel}")
{
    public int MaxLevel { get; } = maxLevel;
}
