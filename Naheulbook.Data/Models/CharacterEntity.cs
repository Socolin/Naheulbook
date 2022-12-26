using System;
using System.Collections.Generic;
using Naheulbook.Data.Extensions;

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

        private UserEntity? _owner;
        public UserEntity Owner { get => _owner.ThrowIfNotLoaded(); set => _owner = value; }

        public Guid OriginId { get; set; }
        private OriginEntity? _origin;
        public OriginEntity Origin { get => _origin.ThrowIfNotLoaded(); set => _origin = value; }

        public int? GroupId { get; set; }
        private GroupEntity? _group;
        public GroupEntity? Group { get => _group.ThrowIfNotLoadedAndNotNull(GroupId); set => _group = value; }

        public int? TargetedCharacterId { get; set; }
        private CharacterEntity? _targetedCharacter;
        public CharacterEntity? TargetedCharacter { get => _targetedCharacter.ThrowIfNotLoadedAndNotNull(TargetedCharacterId); set => _targetedCharacter = value; }

        public int? TargetedMonsterId { get; set; }
        private MonsterEntity? _targetedMonster;
        public MonsterEntity? TargetedMonster { get => _targetedMonster.ThrowIfNotLoadedAndNotNull(TargetedMonsterId); set => _targetedMonster = value; }

        private ICollection<CharacterJobEntity>? _jobs;
        public ICollection<CharacterJobEntity> Jobs { get => _jobs.ThrowIfNotLoaded(); set => _jobs = value; }

        private ICollection<CharacterModifierEntity>? _modifiers;
        public ICollection<CharacterModifierEntity> Modifiers { get => _modifiers.ThrowIfNotLoaded(); set => _modifiers = value; }

        private ICollection<CharacterSkillEntity>? _skills;
        public ICollection<CharacterSkillEntity> Skills { get => _skills.ThrowIfNotLoaded(); set => _skills = value; }

        private ICollection<CharacterSpecialityEntity>? _specialities;
        public ICollection<CharacterSpecialityEntity> Specialities { get => _specialities.ThrowIfNotLoaded(); set => _specialities = value; }

        private ICollection<ItemEntity>? _items;
        public ICollection<ItemEntity> Items { get => _items.ThrowIfNotLoaded(); set => _items = value; }

        private ICollection<GroupInviteEntity>? _invites;
        public ICollection<GroupInviteEntity> Invites { get => _invites.ThrowIfNotLoaded(); set => _invites = value; }

        private ICollection<CharacterHistoryEntryEntity>? _historyEntries;
        public ICollection<CharacterHistoryEntryEntity> HistoryEntries { get => _historyEntries.ThrowIfNotLoaded(); set => _historyEntries = value; }

        public void AddHistoryEntry(CharacterHistoryEntryEntity entry)
        {
            if (_historyEntries == null)
                HistoryEntries = new List<CharacterHistoryEntryEntity>();
            HistoryEntries.Add(entry);
        }
    }
}