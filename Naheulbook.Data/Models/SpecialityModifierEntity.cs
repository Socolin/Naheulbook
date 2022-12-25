using System;

namespace Naheulbook.Data.Models
{
    public class SpecialityModifierEntity
    {
        public int Id { get; set; }
        public string Stat { get; set; } = null!;
        public int Value { get; set; }

        public Guid SpecialityId { get; set; }
        public SpecialityEntity Speciality { get; set; } = null!;
    }
}