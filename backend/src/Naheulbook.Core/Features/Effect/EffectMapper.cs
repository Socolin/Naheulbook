using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Effect;

public interface IEffectMapper
{
    EffectResponse ToResponse(EffectEntity effect);
    EffectTypeResponse ToResponse(EffectTypeEntity effectType);
    EffectSubCategoryResponse ToResponse(EffectSubCategoryEntity effectSubCategory);
    StatModifierResponse ToResponse(EffectModifierEntity effectModifier);
}

public class EffectMapper : IEffectMapper
{
    public EffectResponse ToResponse(EffectEntity effect)
    {
        return new EffectResponse
        {
            Id = effect.Id,
            Name = effect.Name,
            Description = effect.Description,
            DurationType = effect.DurationType,
            Duration = effect.Duration,
            CombatCount = effect.CombatCount,
            LapCount = effect.LapCount,
            TimeDuration = effect.TimeDuration,
            Dice = effect.Dice,
            SubCategoryId = effect.SubCategoryId,
            Modifiers = effect.Modifiers.OrderBy(m => m.StatName).Select(ToResponse).ToList(),
        };
    }

    public EffectTypeResponse ToResponse(EffectTypeEntity effectType)
    {
        return new EffectTypeResponse
        {
            Id = effectType.Id,
            Name = effectType.Name,
            SubCategories = effectType.SubCategories.OrderBy(x => x.Id).Select(ToResponse).ToList(),
        };
    }

    public EffectSubCategoryResponse ToResponse(EffectSubCategoryEntity effectSubCategory)
    {
        return new EffectSubCategoryResponse
        {
            Id = effectSubCategory.Id,
            Name = effectSubCategory.Name,
            DiceCount = effectSubCategory.DiceCount,
            DiceSize = effectSubCategory.DiceSize,
            Note = effectSubCategory.Note,
            TypeId = effectSubCategory.TypeId,
        };
    }

    public StatModifierResponse ToResponse(EffectModifierEntity effectModifier)
    {
        return new StatModifierResponse
        {
            Stat = effectModifier.StatName,
            Type = effectModifier.Type,
            Value = effectModifier.Value,
        };
    }
}
