namespace Naheulbook.Data.Models
{
    public class MonsterLocation
    {
        public int MonsterTemplateId { get; set; }
        public MonsterTemplate MonsterTemplate { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}