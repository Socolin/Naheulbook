namespace Naheulbook.Data.Models
{
    public class CharacterJob
    {
        public int CharacterId { get; set; }
        public Character Character { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; }

        public int Order { get; set; }
    }
}