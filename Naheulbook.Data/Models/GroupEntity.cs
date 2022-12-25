using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class GroupEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Data { get; set; }
        public string? Config { get; set; }

        public int? CombatLootId { get; set; }
        public LootEntity? CombatLoot { get; set; }

        public int MasterId { get; set; }
        public UserEntity Master { get; set; } = null!;

        public ICollection<LootEntity> Loots { get; set; } = null!;
        public ICollection<MonsterEntity> Monsters { get; set; } = null!;
        public ICollection<CharacterEntity> Characters { get; set; } = null!;
        public ICollection<GroupInviteEntity> Invites { get; set; } = null!;
        public ICollection<EventEntity> Events { get; set; } = null!;
        public ICollection<GroupHistoryEntryEntity> HistoryEntries { get; set; } = null!;
        public ICollection<NpcEntity> Npcs { get; set; } = null!;

        public void AddHistoryEntry(GroupHistoryEntryEntity entry)
        {
            if (HistoryEntries == null)
                HistoryEntries = new List<GroupHistoryEntryEntity>();
            HistoryEntries.Add(entry);
        }
    }
}