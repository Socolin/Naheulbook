using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Speciality
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Flags { get; set; }

        public int JobId { get; set; }
        public virtual Job Job { get; set; } = null!;

        public ICollection<SpecialityModifier> Modifiers { get; set; } = null!;
        public ICollection<SpecialitySpecial> Specials { get; set; } = null!;
    }
}