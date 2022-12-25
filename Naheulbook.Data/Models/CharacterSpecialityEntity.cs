using System;

namespace Naheulbook.Data.Models
{
    public class CharacterSpecialityEntity
    {
        public int CharacterId { get; set; }
        public CharacterEntity Character { get; set; } = null!;

        public Guid SpecialityId { get; set; }
        public SpecialityEntity Speciality { get; set; } = null!;
    }
}