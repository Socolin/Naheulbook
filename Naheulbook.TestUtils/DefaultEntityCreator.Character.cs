using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public Character CreateCharacter(int ownerId, Origin origin, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Character
            {
                Name = $"some-character-name-{suffix}",
                Sex = "Homme",
                IsActive = true,
                IsNpc = false,

                Ad = 3,
                Cha = 4,
                Cou = 5,
                Fo = 6,
                Int = 7,

                Color = "DADADA",

                Level = 1,
                Experience = 42,
                FatePoint = 1,

                Ev = 12,
                Ea = 8,

                StatBonusAd = "PRD",

                OriginId = origin.Id,
                OwnerId = ownerId
            };
        }

        public CharacterModifier CreateCharacterModifier(IList<CharacterModifierValue> values, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new CharacterModifier
            {
                Name = $"some-character-name-{suffix}",
                Description = $"some-description",
                IsActive = true,
                Reusable = false,
                Permanent = false,
                Values = values
            };
        }

        public CharacterModifierValue CreateCharacterModifierValue(Stat stat, short value)
        {
            return new CharacterModifierValue
            {
                StatName = stat.Name,
                Value = value,
                Type = "ADD"
            };
        }
    }
}