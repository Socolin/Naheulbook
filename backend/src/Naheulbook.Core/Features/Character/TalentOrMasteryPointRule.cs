namespace Naheulbook.Core.Features.Character;

/// <summary>
/// At each odd level from level 3 (3, 5, 7...), the character gets 1 point to learn either a talent or a mastery.
/// </summary>
public static class TalentOrMasteryPointRule
{
    public static bool GivesPointAtLevel(int level) => level >= 3 && level % 2 == 1;
}
