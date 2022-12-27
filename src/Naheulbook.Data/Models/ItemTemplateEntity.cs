using System;
using System.Collections.Generic;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

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
    private ItemTemplateSubCategoryEntity? _subCategory;
    public ItemTemplateSubCategoryEntity SubCategory { get => _subCategory.ThrowIfNotLoaded(); set => _subCategory = value; }

    public int? SourceUserId { get; set; }
    private UserEntity? _sourceUser;
    public UserEntity? SourceUser { get => _sourceUser.ThrowIfNotLoadedAndNotNull(SourceUserId); set => _sourceUser = value; }

    private ICollection<ItemTemplateModifierEntity>? _modifiers;
    public ICollection<ItemTemplateModifierEntity> Modifiers { get => _modifiers.ThrowIfNotLoaded(); set => _modifiers = value; }

    private ICollection<ItemTemplateRequirementEntity>? _requirements;
    public ICollection<ItemTemplateRequirementEntity> Requirements { get => _requirements.ThrowIfNotLoaded(); set => _requirements = value; }

    private ICollection<ItemTemplateSkillModifierEntity>? _skillModifiers;
    public ICollection<ItemTemplateSkillModifierEntity> SkillModifiers { get => _skillModifiers.ThrowIfNotLoaded(); set => _skillModifiers = value; }

    private ICollection<ItemTemplateSlotEntity>? _slots;
    public ICollection<ItemTemplateSlotEntity> Slots { get => _slots.ThrowIfNotLoaded(); set => _slots = value; }

    private ICollection<ItemTemplateSkillEntity>? _skills;
    public ICollection<ItemTemplateSkillEntity> Skills { get => _skills.ThrowIfNotLoaded(); set => _skills = value; }

    private ICollection<ItemTemplateUnSkillEntity>? _unSkills;
    public ICollection<ItemTemplateUnSkillEntity> UnSkills { get => _unSkills.ThrowIfNotLoaded(); set => _unSkills = value; }
}