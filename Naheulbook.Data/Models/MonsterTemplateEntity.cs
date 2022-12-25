using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MonsterTemplateEntity
    {
        public int Id { get; set; }

        public string Data { get; set; } = null!;
        public string Name { get; set; } = null!;

        public int SubCategoryId { get; set; }
        public MonsterSubCategoryEntity SubCategory { get; set; } = null!;

        public ICollection<MonsterTemplateInventoryElementEntity> Items { get; set; } = null!;
    }
}