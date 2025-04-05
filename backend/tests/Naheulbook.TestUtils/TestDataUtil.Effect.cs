using System;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddEffectType(Action<EffectTypeEntity> customizer = null)
    {
        return AddEffectType(out _);
    }

    public TestDataUtil AddEffectType(out EffectTypeEntity effectType, Action<EffectTypeEntity> customizer = null)
    {
        effectType = new EffectTypeEntity
        {
            Name = RngUtil.GetRandomString("some-effect-type-name"),
        };

        return SaveEntity(effectType, customizer);
    }

    public TestDataUtil AddEffectSubCategory(Action<EffectSubCategoryEntity> customizer = null)
    {
        return AddEffectSubCategory(out _, customizer);
    }

    public TestDataUtil AddEffectSubCategory(out EffectSubCategoryEntity effectSubCategory, Action<EffectSubCategoryEntity> customizer = null)
    {
        var effectType = GetLast<EffectTypeEntity>();

        effectSubCategory = new EffectSubCategoryEntity
        {
            Name = RngUtil.GetRandomString("some-effect-sub-category-name"),
            Note = RngUtil.GetRandomString("some-effect-name"),
            DiceCount = 1,
            DiceSize = 20,
            TypeId = effectType.Id,
        };

        return SaveEntity(effectSubCategory, customizer);
    }

    public TestDataUtil AddEffect(Action<EffectEntity> customizer = null)
    {
        return AddEffect(out _, customizer);
    }

    public TestDataUtil AddEffect(out EffectEntity effect, Action<EffectEntity> customizer = null)
    {
        var effectSubCategory = GetLast<EffectSubCategoryEntity>();

        effect = new EffectEntity
        {
            SubCategoryId = effectSubCategory.Id,
            Description = RngUtil.GetRandomString("some-description"),
            Dice = 4,
            Name = RngUtil.GetRandomString("some-effect-name"),
            DurationType = "combat",
            CombatCount = 2,
        };

        return SaveEntity(effect, customizer);
    }

    public TestDataUtil AddEffectModifier(Action<EffectModifierEntity> customizer = null)
    {
        return AddEffectModifier(out _, customizer);
    }

    public TestDataUtil AddEffectModifier(out EffectModifierEntity effectModifier, Action<EffectModifierEntity> customizer = null)
    {
        var stat = GetLastIfExists<StatEntity>();
        if (stat == null)
            AddStat(out stat);

        var effect = GetLast<EffectEntity>();
        effectModifier = new EffectModifierEntity
        {
            EffectId = effect.Id,
            StatName = stat.Name,
            Value = 12,
            Type = "ADD",
        };
        return SaveEntity(effectModifier, customizer);
    }
}