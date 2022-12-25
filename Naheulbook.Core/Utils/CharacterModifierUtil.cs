using Naheulbook.Core.Exceptions;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Utils
{
    public interface ICharacterModifierUtil
    {
        void ToggleModifier(CharacterEntity character, CharacterModifierEntity modifier);
    }

    public class CharacterModifierUtil : ICharacterModifierUtil
    {
        private readonly ICharacterHistoryUtil _characterHistoryUtil;

        public CharacterModifierUtil(ICharacterHistoryUtil characterHistoryUtil)
        {
            _characterHistoryUtil = characterHistoryUtil;
        }

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

                character.AddHistoryEntry(_characterHistoryUtil.CreateLogActiveModifier(character.Id, modifier.Id));
            }
            else
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogDisableModifier(character.Id, modifier.Id));
            }
        }
    }
}