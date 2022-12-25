using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MonsterSubCategoryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int TypeId { get; set; }
        public MonsterTypeEntity Type { get; set; } = null!;

        public ICollection<MonsterTemplateEntity> MonsterTemplates { get; set; } = null!;
    }
}