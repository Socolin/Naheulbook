using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class CharacterSpecialityEntity
    {
        public int CharacterId { get; set; }
        private CharacterEntity? _character;
        public CharacterEntity Character { get => _character.ThrowIfNotLoaded(); set => _character = value; }

        public Guid SpecialityId { get; set; }
        private SpecialityEntity? _speciality;
        public SpecialityEntity Speciality { get => _speciality.ThrowIfNotLoaded(); set => _speciality = value; }
    }
}