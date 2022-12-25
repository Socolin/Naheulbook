using System;
using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Data.Models
{
    public class SpecialityEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Flags { get; set; }

        public Guid JobId { get; set; }
        public virtual JobEntity Job { get; set; } = null!;

        public ICollection<SpecialityModifierEntity> Modifiers { get; set; } = null!;
        public ICollection<SpecialitySpecialEntity> Specials { get; set; } = null!;
    }
}