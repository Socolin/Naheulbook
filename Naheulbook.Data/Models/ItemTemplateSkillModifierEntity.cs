using System;

namespace Naheulbook.Data.Models
{
    public class ItemTemplateSkillModifierEntity
    {
        public int Id { get; set; }

        public short Value { get; set; }

        public Guid ItemTemplateId { get; set; }
        public ItemTemplateEntity ItemTemplate { get; set; } = null!;

        public Guid SkillId { get; set; }
        public SkillEntity Skill { get; set; } = null!;
    }
}