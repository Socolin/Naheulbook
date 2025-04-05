

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Character;

public class InvalidTargetLevelUpRequestedException(int currentLevel, int targetLevel) : Exception
{
    public int CurrentLevel { get; } = currentLevel;
    public int TargetLevel { get; } = targetLevel;
}