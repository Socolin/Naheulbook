using System;

namespace Naheulbook.Data.Models
{
    public class CharacterHistoryEntryEntity : IHistoryEntry
    {
        public int Id { get; set; }
        public string Action { get; set; } = null!;
        public string? Data { get; set; }
        public DateTime Date { get; set; }
        public bool Gm { get; set; }
        public string? Info { get; set; }

        public int? EffectId { get; set; }
        public EffectEntity? Effect { get; set; }

        public int? CharacterModifierId { get; set; }
        public CharacterModifierEntity? CharacterModifier { get; set; }

        public int? ItemId { get; set; }
        public ItemEntity? Item { get; set; }

        public int CharacterId { get; set; }
        public CharacterEntity Character { get; set; } = null!;
    }
}