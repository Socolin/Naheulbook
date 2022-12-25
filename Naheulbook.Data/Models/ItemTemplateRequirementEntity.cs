using System;

namespace Naheulbook.Data.Models
{
    public class ItemTemplateRequirementEntity
    {
        public int Id { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }

        public Guid ItemTemplateId { get; set; }
        public ItemTemplateEntity ItemTemplate { get; set; } = null!;

        public string StatName { get; set; } = null!;
        public StatEntity Stat { get; set; } = null!;
    }
}