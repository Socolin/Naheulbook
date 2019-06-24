using System;

namespace Naheulbook.Data.Models
{
    public class CharacterHistoryEntry : IHistoryEntry
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }
        public DateTime Date { get; set; }
        public bool Gm { get; set; }
        public string Info { get; set; }

        public int? EffectId { get; set; }
        public Effect Effect { get; set; }

        public int? CharacterModifierId { get; set; }
        public CharacterModifier CharacterModifier { get; set; }

        public int? ItemId { get; set; }
        public Item Item { get; set; }

        public int CharacterId { get; set; }
        public Character Character { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}