using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class EventEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long Timestamp { get; set; }

        public int GroupId { get; set; }
        private GroupEntity? _group;
        public GroupEntity Group { get => _group.ThrowIfNotLoaded(); set => _group = value; }
    }
}