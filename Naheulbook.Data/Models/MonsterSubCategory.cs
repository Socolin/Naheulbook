using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MonsterSubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int TypeId { get; set; }
        public MonsterType Type { get; set; } = null!;

        public ICollection<MonsterTemplate> MonsterTemplates { get; set; } = null!;
    }
}