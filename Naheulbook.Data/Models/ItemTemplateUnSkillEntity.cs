using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class ItemTemplateUnSkillEntity
    {
        public Guid ItemTemplateId { get; set; }
        public ItemTemplateEntity ItemTemplate { get; set; } = null!;

        public Guid SkillId { get; set; }
        public SkillEntity Skill { get; set; } = null!;
    }
}