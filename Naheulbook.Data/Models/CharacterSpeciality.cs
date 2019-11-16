using System;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class CharacterSpeciality
    {
        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;

        public Guid SpecialityId { get; set; }
        public Speciality Speciality { get; set; } = null!;
    }
}