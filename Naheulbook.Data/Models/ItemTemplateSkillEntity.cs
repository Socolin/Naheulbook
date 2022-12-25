using System;

namespace Naheulbook.Data.Models
{
    public class ItemTemplateSkillEntity
    {
        public Guid ItemTemplateId { get; set; }
        public ItemTemplateEntity ItemTemplate { get; set; } = null!;

        public Guid SkillId { get; set; }
        public SkillEntity Skill { get; set; } = null!;
    }
}