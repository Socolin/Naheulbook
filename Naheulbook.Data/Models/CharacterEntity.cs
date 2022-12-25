using System;
using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class CharacterEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Sex { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool IsNpc { get; set; }
        public string Color { get; set; } = null!;

        public int Ad { get; set; }
        public int Cha { get; set; }
        public int Cou { get; set; }
        public int Fo { get; set; }
        public int Int { get; set; }

        public int? Ev { get; set; }
        public int? Ea { get; set; }

        public string? GmData { get; set; }
        public short FatePoint { get; set; }
        public string? StatBonusAd { get; set; }

        public string? Notes { get; set; }

        public int Level { get; set; }
        public int Experience { get; set; }

        public int OwnerId { get; set; }
        public UserEntity Owner { get; set; } = null!;

        public Guid OriginId { get; set; }
        public OriginEntity Origin { get; set; } = null!;

        public int? GroupId { get; set; }
        public GroupEntity? Group { get; set; }

        public int? TargetedCharacterId { get; set; }
        public CharacterEntity? TargetedCharacter { get; set; }

        public int? TargetedMonsterId { get; set; }
        public MonsterEntity? TargetedMonster { get; set; }

        public ICollection<CharacterJobEntity> Jobs { get; set; } = null!;
        public ICollection<CharacterModifierEntity> Modifiers { get; set; } = null!;
        public ICollection<CharacterSkillEntity> Skills { get; set; } = null!;
        public ICollection<CharacterSpecialityEntity> Specialities { get; set; } = null!;
        public ICollection<ItemEntity> Items { get; set; } = null!;
        public ICollection<GroupInviteEntity> Invites { get; set; } = null!;
        public ICollection<CharacterHistoryEntryEntity> HistoryEntries { get; set; } = null!;

        public void AddHistoryEntry(CharacterHistoryEntryEntity entry)
        {
            if (HistoryEntries == null)
                HistoryEntries = new List<CharacterHistoryEntryEntity>();
            HistoryEntries.Add(entry);
        }
    }
}