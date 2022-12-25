using System;

namespace Naheulbook.Data.Models
{
    public class MonsterTemplateInventoryElementEntity
    {
        public int Id { get; set; }
        public float Chance { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }

        public Guid ItemTemplateId { get; set; }
        public ItemTemplateEntity ItemTemplate { get; set; } = null!;

        public int MonsterTemplateId { get; set; }
        public MonsterTemplateEntity MonsterTemplate { get; set; } = null!;
    }
}