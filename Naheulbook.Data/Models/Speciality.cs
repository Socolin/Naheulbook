using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Speciality
    {
        public Speciality()
        {
            Modifiers = new HashSet<SpecialityModifier>();
            Specials = new HashSet<SpecialitySpecial>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Flags { get; set; }

        public long JobId { get; set; }
        public virtual Job Job { get; set; }

        public ICollection<SpecialityModifier> Modifiers { get; set; }
        public ICollection<SpecialitySpecial> Specials { get; set; }
    }
}