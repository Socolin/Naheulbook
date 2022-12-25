using System;
using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Data.Models
{
    public class ItemTemplateEntity
    {
        public const string OfficialSourceValue = "official";
        public const string PrivateSourceValue = "private";
        public const string CommunitySourceValue = "community";

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? CleanName { get; set; }
        public string? TechName { get; set; }
        public string Source { get; set; } = null!;
        public string? SourceUserNameCache { get; set; }
        public string? Data { get; set; }

        public int SubCategoryId { get; set; }
        public ItemTemplateSubCategoryEntity SubCategory { get; set; } = null!;

        public int? SourceUserId { get; set; }
        public UserEntity? SourceUser { get; set; }

        public ICollection<ItemTemplateModifierEntity> Modifiers { get; set; } = null!;
        public ICollection<ItemTemplateRequirementEntity> Requirements { get; set; } = null!;
        public ICollection<ItemTemplateSkillModifierEntity> SkillModifiers { get; set; } = null!;
        public ICollection<ItemTemplateSlotEntity> Slots { get; set; } = null!;
        public ICollection<ItemTemplateSkillEntity> Skills { get; set; } = null!;
        public ICollection<ItemTemplateUnSkillEntity> UnSkills { get; set; } = null!;
    }
}