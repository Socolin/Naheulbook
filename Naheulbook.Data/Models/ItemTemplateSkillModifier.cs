using System;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Data.Models
{
    public class ItemTemplateSkillModifier
    {
        public int Id { get; set; }

        public short Value { get; set; }

        public int ItemTemplateId { get; set; }
        public ItemTemplate ItemTemplate { get; set; } = null!;

        public Guid SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}