using System;

namespace Naheulbook.Data.Models
{
    public class GroupHistoryEntryEntity : IHistoryEntry
    {
        public int Id { get; set; }
        public string Action { get; set; } = null!;
        public string? Data { get; set; }
        public DateTime Date { get; set; }
        public bool Gm { get; set; }
        public string? Info { get; set; }

        public int GroupId { get; set; }
        public GroupEntity Group { get; set; } = null!;
    }
}