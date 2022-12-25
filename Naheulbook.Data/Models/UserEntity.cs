using System;
using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? HashedPassword { get; set; }

        public string? ActivationCode { get; set; }

        public bool Admin { get; set; }

        public string? FbId { get; set; }

        public string? GoogleId { get; set; }

        public string? MicrosoftId { get; set; }

        public string? TwitterId { get; set; }

        public string? DisplayName { get; set; }

        public DateTime? ShowInSearchUntil { get; set; }

        public ICollection<GroupEntity> Groups { get; set; } = null!;
        public ICollection<CharacterEntity> Characters { get; set; } = null!;
    }
}