using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MonsterTemplate
    {
        public int Id { get; set; }

        public string Data { get; set; }
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public MonsterCategory Category { get; set; }

        public ICollection<MonsterTemplateSimpleInventory> Items { get; set; }
        public ICollection<MonsterLocation> Locations { get; set; }
    }
}