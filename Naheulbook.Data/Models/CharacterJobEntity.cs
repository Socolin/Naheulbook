using System;

namespace Naheulbook.Data.Models
{
    public class CharacterJobEntity
    {
        public int CharacterId { get; set; }
        public CharacterEntity Character { get; set; } = null!;

        public Guid JobId { get; set; }
        public JobEntity Job { get; set; } = null!;

        public int Order { get; set; }
    }
}