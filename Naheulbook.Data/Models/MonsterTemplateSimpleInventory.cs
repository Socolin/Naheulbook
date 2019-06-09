namespace Naheulbook.Data.Models
{
    public class MonsterTemplateSimpleInventory
    {
        public int Id { get; set; }
        public float Chance { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }

        public int ItemTemplateId { get; set; }
        public ItemTemplate ItemTemplate { get; set; }

        public int MonsterTemplateId { get; set; }
        public MonsterTemplate MonsterTemplate { get; set; }
    }
}