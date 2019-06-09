using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MonsterCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int TypeId { get; set; }
        public MonsterType Type { get; set; }

        public ICollection<MonsterTemplate> MonsterTemplates { get; set; }
    }
}