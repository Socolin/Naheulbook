using System;

namespace Naheulbook.Data.Models
{
    public class SpecialitySpecialEntity
    {
        public int Id { get; set; }
        public bool IsBonus { get; set; }
        public string Description { get; set; } = null!;
        public string? Flags { get; set; }

        public Guid SpecialityId { get; set; }
        public SpecialityEntity Speciality { get; set; } = null!;
    }
}