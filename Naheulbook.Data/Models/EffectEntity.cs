using System.Collections.Generic;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

public class EffectTypeEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    private ICollection<EffectSubCategoryEntity>? _subCategories;
    public ICollection<EffectSubCategoryEntity> SubCategories { get => _subCategories.ThrowIfNotLoaded(); set => _subCategories = value; }
}

public class EffectSubCategoryEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public short DiceCount { get; set; }
    public short DiceSize { get; set; }
    public string? Note { get; set; }

    public int TypeId { get; set; }
    private EffectTypeEntity? _type;
    public EffectTypeEntity Type { get => _type.ThrowIfNotLoaded(); set => _type = value; }

    private ICollection<EffectEntity>? _effects;
    public ICollection<EffectEntity> Effects { get => _effects.ThrowIfNotLoaded(); set => _effects = value; }
}

public class EffectEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string DurationType { get; set; } = null!;
    public string? Duration { get; set; }
    public int? CombatCount { get; set; }
    public int? LapCount { get; set; }
    public int? TimeDuration { get; set; }
    public short? Dice { get; set; }

    public int SubCategoryId { get; set; }
    private EffectSubCategoryEntity? _subCategory;
    public EffectSubCategoryEntity SubCategory { get => _subCategory.ThrowIfNotLoaded(); set => _subCategory = value; }

    private ICollection<EffectModifierEntity>? _modifiers;
    public ICollection<EffectModifierEntity> Modifiers { get => _modifiers.ThrowIfNotLoaded(); set => _modifiers = value; }
}


public class EffectModifierEntity
{
    public short Value { get; set; }
    public string Type { get; set; } = null!;

    public int EffectId { get; set; }
    private EffectEntity? _effect;
    public EffectEntity Effect { get => _effect.ThrowIfNotLoaded(); set => _effect = value; }

    public string StatName { get; set; } = null!;
    private StatEntity? _stat;
    public StatEntity Stat { get => _stat.ThrowIfNotLoaded(); set => _stat = value; }
}