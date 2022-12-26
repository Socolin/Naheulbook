using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public CharacterEntity CreateCharacter(int ownerId, OriginEntity origin, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new CharacterEntity
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

                Notes = "some-notes",

                Level = 1,
                Experience = 42,
                FatePoint = 1,

                Ev = 12,
                Ea = 8,

                StatBonusAd = "PRD",

                OriginId = origin.Id,
                OwnerId = ownerId,

                Jobs = new List<CharacterJobEntity>(),
                Modifiers = new List<CharacterModifierEntity>(),
                Skills = new List<CharacterSkillEntity>(),
                Specialities = new List<CharacterSpecialityEntity>(),
                Items = new List<ItemEntity>(),
                Invites = new List<GroupInviteEntity>(),
                HistoryEntries = new List<CharacterHistoryEntryEntity>(),
            };
        }

        public CharacterModifierEntity CreateCharacterModifier(IList<CharacterModifierValueEntity> values, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new CharacterModifierEntity
            {
                Name = $"some-character-name-{suffix}",
                Description = "some-description",
                DurationType = "forever",
                IsActive = true,
                Reusable = false,
                Permanent = false,
                Values = values
            };
        }

        public CharacterModifierValueEntity CreateCharacterModifierValue(StatEntity stat, short value)
        {
            return new CharacterModifierValueEntity
            {
                StatName = stat.Name,
                Value = value,
                Type = "ADD"
            };
        }

        public CharacterHistoryEntryEntity CreateCharacterHistoryEntry(CharacterEntity character, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new CharacterHistoryEntryEntity
            {
                Data = "{}",
                Gm = false,
                Date = new DateTime(2019, 10, 5, 5, 7, 8, DateTimeKind.Utc),
                CharacterId = character.Id,
                Action = $"some-character-history-action-{suffix}",
                Info = $"some-info-{suffix}"
            };
        }
    }
}