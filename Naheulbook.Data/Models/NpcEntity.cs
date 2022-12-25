namespace Naheulbook.Data.Models
{
    public class NpcEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Data { get; set; } = null!;

        public int GroupId { get; set; }
        public GroupEntity Group { get; set; } = null!;
    }
}