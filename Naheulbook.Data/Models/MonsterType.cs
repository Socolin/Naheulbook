using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MonsterType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<MonsterCategory> Categories { get; set; } = null!;
    }
}