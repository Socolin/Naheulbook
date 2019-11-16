using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class CharacterJob
    {
        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;

        public Guid JobId { get; set; }
        public Job Job { get; set; } = null!;

        public int Order { get; set; }
    }
}