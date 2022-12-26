using System.Collections.Generic;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class MonsterSubCategoryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int TypeId { get; set; }
        private MonsterTypeEntity? _type;
        public MonsterTypeEntity Type { get => _type.ThrowIfNotLoaded(); set => _type = value; }

        private ICollection<MonsterTemplateEntity>? _monsterTemplates;
        public ICollection<MonsterTemplateEntity> MonsterTemplates { get => _monsterTemplates.ThrowIfNotLoaded(); set => _monsterTemplates = value; }
    }
}