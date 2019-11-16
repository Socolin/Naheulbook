using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class SpecialityModifier
    {
        public int Id { get; set; }
        public string Stat { get; set; } = null!;
        public int Value { get; set; }

        public Guid SpecialityId { get; set; }
        public Speciality Speciality { get; set; } = null!;
    }
}