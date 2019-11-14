using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class ItemTemplate
    {
        public const string OfficialSourceValue = "official";
        public const string PrivateSourceValue = "private";
        public const string CommunitySourceValue = "community";

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? CleanName { get; set; }
        public string? TechName { get; set; }
        public string Source { get; set; } = null!;
        public string? SourceUserNameCache { get; set; }
        public string? Data { get; set; }

        public int SubCategoryId { get; set; }
        public ItemTemplateSubCategory SubCategory { get; set; } = null!;

        public int? SourceUserId { get; set; }
        public User? SourceUser { get; set; }

        public ICollection<ItemTemplateModifier> Modifiers { get; set; } = null!;
        public ICollection<ItemTemplateRequirement> Requirements { get; set; } = null!;
        public ICollection<ItemTemplateSkillModifier> SkillModifiers { get; set; } = null!;
        public ICollection<ItemTemplateSlot> Slots { get; set; } = null!;
        public ICollection<ItemTemplateSkill> Skills { get; set; } = null!;
        public ICollection<ItemTemplateUnSkill> UnSkills { get; set; } = null!;
    }
}