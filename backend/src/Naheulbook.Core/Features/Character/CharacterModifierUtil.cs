using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterModifierUtil
{
    void ToggleModifier(CharacterEntity character, CharacterModifierEntity modifier);
}

public class CharacterModifierUtil(ICharacterHistoryUtil characterHistoryUtil) : ICharacterModifierUtil
{
    public void ToggleModifier(CharacterEntity character, CharacterModifierEntity modifier)
    {
        if (!modifier.Reusable)
            throw new CharacterModifierNotReusableException(modifier.Id);

        modifier.IsActive = !modifier.IsActive;
        if (modifier.IsActive)
        {
            modifier.CurrentCombatCount = modifier.CombatCount;
            modifier.CurrentLapCount = modifier.LapCount;
            modifier.CurrentTimeDuration = modifier.TimeDuration;

            character.AddHistoryEntry(characterHistoryUtil.CreateLogActiveModifier(character.Id, modifier.Id));
        }
        else
        {
            character.AddHistoryEntry(characterHistoryUtil.CreateLogDisableModifier(character.Id, modifier.Id));
        }
    }
}