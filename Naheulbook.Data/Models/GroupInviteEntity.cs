namespace Naheulbook.Data.Models
{
    public class GroupInviteEntity
    {
        public bool FromGroup { get; set; }

        public int GroupId { get; set; }
        public GroupEntity Group { get; set; } = null!;

        public int CharacterId { get; set; }
        public CharacterEntity Character { get; set; } = null!;
    }
}