using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MonsterTemplate
    {
        public int Id { get; set; }

        public string Data { get; set; } = null!;
        public string Name { get; set; } = null!;

        public int SubCategoryId { get; set; }
        public MonsterSubCategory SubCategory { get; set; } = null!;

        public ICollection<MonsterTemplateSimpleInventory> Items { get; set; } = null!;
    }
}