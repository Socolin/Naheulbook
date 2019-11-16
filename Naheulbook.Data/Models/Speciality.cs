using System;
using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Data.Models
{
    public class Speciality
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Flags { get; set; }

        public Guid JobId { get; set; }
        public virtual Job Job { get; set; } = null!;

        public ICollection<SpecialityModifier> Modifiers { get; set; } = null!;
        public ICollection<SpecialitySpecial> Specials { get; set; } = null!;
    }
}