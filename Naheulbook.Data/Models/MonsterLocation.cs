namespace Naheulbook.Data.Models
{
    public class MonsterLocation
    {
        public int MonsterTemplateId { get; set; }
        public MonsterTemplate MonsterTemplate { get; set; } = null!;

        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;
    }
}