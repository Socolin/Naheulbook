using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class SpecialityModifierEntity
    {
        public int Id { get; set; }
        public string Stat { get; set; } = null!;
        public int Value { get; set; }

        public Guid SpecialityId { get; set; }
        private SpecialityEntity? _speciality;
        public SpecialityEntity Speciality { get => _speciality.ThrowIfNotLoaded(); set => _speciality = value; }
    }
}