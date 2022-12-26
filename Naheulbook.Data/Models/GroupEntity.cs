using System.Collections.Generic;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class GroupEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Data { get; set; }
        public string? Config { get; set; }

        public int? CombatLootId { get; set; }
        private LootEntity? _combatLoot;
        public LootEntity? CombatLoot { get => _combatLoot.ThrowIfNotLoadedAndNotNull(CombatLootId); set => _combatLoot = value; }

        public int MasterId { get; set; }
        private UserEntity? _master;
        public UserEntity Master { get => _master.ThrowIfNotLoaded(); set => _master = value; }

        private ICollection<LootEntity>? _loots;
        public ICollection<LootEntity> Loots { get => _loots.ThrowIfNotLoaded(); set => _loots = value; }

        private ICollection<MonsterEntity>? _monsters;
        public ICollection<MonsterEntity> Monsters { get => _monsters.ThrowIfNotLoaded(); set => _monsters = value; }

        private ICollection<CharacterEntity>? _characters;
        public ICollection<CharacterEntity> Characters { get => _characters.ThrowIfNotLoaded(); set => _characters = value; }

        private ICollection<GroupInviteEntity>? _invites;
        public ICollection<GroupInviteEntity> Invites { get => _invites.ThrowIfNotLoaded(); set => _invites = value; }

        private ICollection<EventEntity>? _events;
        public ICollection<EventEntity> Events { get => _events.ThrowIfNotLoaded(); set => _events = value; }

        private ICollection<GroupHistoryEntryEntity>? _historyEntries;
        public ICollection<GroupHistoryEntryEntity> HistoryEntries { get => _historyEntries.ThrowIfNotLoaded(); set => _historyEntries = value; }

        private ICollection<NpcEntity>? _npcs;
        public ICollection<NpcEntity> Npcs { get => _npcs.ThrowIfNotLoaded(); set => _npcs = value; }

        public void AddHistoryEntry(GroupHistoryEntryEntity entry)
        {
            if (_historyEntries == null)
                HistoryEntries = new List<GroupHistoryEntryEntity>();
            HistoryEntries.Add(entry);
        }
    }
}