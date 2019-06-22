namespace Naheulbook.Data.Models
{
    public class GroupInvite
    {
        public bool FromGroup { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int CharacterId { get; set; }
        public Character Character { get; set; }
    }
}