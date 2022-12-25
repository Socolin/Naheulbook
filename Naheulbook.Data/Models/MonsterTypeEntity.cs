using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MonsterTypeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<MonsterSubCategoryEntity> SubCategories { get; set; } = null!;
    }
}