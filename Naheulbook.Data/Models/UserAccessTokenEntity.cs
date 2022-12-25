using System;

namespace Naheulbook.Data.Models
{
    public class UserAccessTokenEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Key { get; set; } = null!;
        public DateTimeOffset DateCreated { get; set; }

        public int UserId { get; set; }
        public UserEntity User { get; set; } = null!;
    }
}