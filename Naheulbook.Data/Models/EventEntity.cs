namespace Naheulbook.Data.Models
{
    public class EventEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long Timestamp { get; set; }

        public int GroupId { get; set; }
        public GroupEntity Group { get; set; } = null!;
    }
}