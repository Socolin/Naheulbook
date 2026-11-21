namespace Naheulbook.Shared.TransientModels;

public class SpellBookData
{
    public string? Description { get; set; }

    /// <summary>
    /// Categories of the book index, in the book order. <c>spells.category</c> is one of them.
    /// </summary>
    public List<string> Categories { get; set; } = [];

    public string? SourcePdf { get; set; }
}

/// <summary>
/// Details of a spell, prodige, ritual or invocation. Every structured block keeps the original <c>Text</c> and its
/// detail lines (<c>Notes</c>) as written in the book, the other fields are parsed from them when possible.
/// </summary>
public class SpellData
{
    /// <summary>
    /// Tags from the book index, e.g. <c>["combat", "assistance"]</c>.
    /// </summary>
    public List<string>? Tags { get; set; }

    public List<string>? Usages { get; set; }
    public SpellCastingTime? CastingTime { get; set; }
    public SpellTime? Duration { get; set; }
    public SpellCost? Cost { get; set; }
    public SpellTest? Test { get; set; }
    public SpellRange? Range { get; set; }
    public SpellText? Area { get; set; }
    public SpellDamage? Damage { get; set; }
    public List<string>? Benefits { get; set; }
    public List<string>? Effects { get; set; }
    public List<string>? Restrictions { get; set; }
    public List<SpellIngredient>? Ingredients { get; set; }
    public string? RitualDetails { get; set; }

    /// <summary>
    /// What the spell allows at each level ("Niveau 2 : Rongeurs…"), as markdown.
    /// </summary>
    public string? LevelDetails { get; set; }

    /// <summary>
    /// "Mot de pouvoir" for mages, "Prière" for priests and paladins.
    /// </summary>
    public List<string>? PowerWord { get; set; }

    /// <summary>
    /// "Caractéristiques" of an invocation.
    /// </summary>
    public List<string>? Characteristics { get; set; }

    public SpellInvocation? Invocation { get; set; }
    public string? CriticalSuccess { get; set; }
    public string? CriticalFailure { get; set; }

    /// <summary>
    /// Sentence about critical success / failure found in the <see cref="Notes"/> (it is also kept in the notes).
    /// </summary>
    public string? CriticalNotes { get; set; }

    public string? Notes { get; set; }
    public SpellSource? Source { get; set; }
}

public class SpellText
{
    public string Text { get; set; } = "";
    public List<string>? Notes { get; set; }
}

public class SpellTime : SpellText
{
    /// <summary>
    /// fixed | immediate | permanent | variable | other
    /// </summary>
    public string? Kind { get; set; }

    public int? Value { get; set; }
    public string? Dice { get; set; }

    /// <summary>
    /// assault | second | minute | hour | day | week | combat
    /// </summary>
    public string? Unit { get; set; }
}

public class SpellCastingTime : SpellTime
{
    /// <summary>
    /// incantation | prayer | ritual
    /// </summary>
    public string? Type { get; set; }
}

public class SpellCost : SpellText
{
    /// <summary>
    /// fixed | variable | per_unit | all_astral_points | other
    /// </summary>
    public string Kind { get; set; } = null!;

    public int? AstralPoints { get; set; }
    public int? MinAstralPoints { get; set; }
    public int? MaxAstralPoints { get; set; }

    /// <summary>
    /// Unit of a <c>per_unit</c> cost: assaut, minute, heure...
    /// </summary>
    public string? Per { get; set; }
}

public class SpellTest : SpellText
{
    /// <summary>
    /// simple | confrontation | none | other
    /// </summary>
    public string Kind { get; set; } = null!;

    /// <summary>
    /// MagiePsy | MagiePhys | INT | CHA | AD | COU | AT | PRD
    /// </summary>
    public string? Stat { get; set; }

    public int? Modifier { get; set; }

    /// <summary>
    /// The test is only needed until this level ("Épreuve nécessaire seulement jusqu’au niveau 8").
    /// </summary>
    public int? RequiredUntilLevel { get; set; }

    public SpellConfrontation? Confrontation { get; set; }

    /// <summary>
    /// The magic resistance rule, when there is one.
    /// </summary>
    public string? MagicResistance { get; set; }
}

public class SpellConfrontation
{
    public string? Caster { get; set; }
    public string? Target { get; set; }
}

public class SpellRange : SpellText
{
    /// <summary>
    /// contact | distance | zone | cone | variable | sight | unlimited | self_area | other
    /// </summary>
    public string Kind { get; set; } = null!;

    public int? Meters { get; set; }
}

public class SpellDamage : SpellText
{
    public string? Dice { get; set; }
    public bool? PerTarget { get; set; }

    /// <summary>
    /// "(fixe)": the INT damage bonus does not apply.
    /// </summary>
    public bool? Fixed { get; set; }

    /// <summary>
    /// "BL": wounds that ignore the armor.
    /// </summary>
    public bool? IgnoresArmor { get; set; }

    public List<SpellDamageByLevel>? ByLevel { get; set; }
}

public class SpellDamageByLevel
{
    public int MinLevel { get; set; }
    public int? MaxLevel { get; set; }
    public string Value { get; set; } = null!;
}

public class SpellIngredient
{
    public string Name { get; set; } = null!;
    public int? Quantity { get; set; }
    public string? Unit { get; set; }
    public string Text { get; set; } = "";
    public Guid? ItemTemplateId { get; set; }
}

public class SpellInvocation
{
    public SpellTime? CallTime { get; set; }
    public SpellTest? CallTest { get; set; }
    public SpellInvocationTest? ControlTest { get; set; }
    public SpellInvocationTest? RevokeTest { get; set; }
    public int? MaxCount { get; set; }
    public SpellText? MaxCountDetails { get; set; }

    /// <summary>
    /// Raw stat block of the creature (ev, int, at, prd, esq, cou, pr, xp, resm).
    /// </summary>
    public Dictionary<string, string>? CreatureStats { get; set; }

    public int? MonsterTemplateId { get; set; }
}

public class SpellInvocationTest : SpellText
{
    public bool Required { get; set; }
    public string? Caster { get; set; }
    public string? Target { get; set; }
}

public class SpellSource
{
    public string Pdf { get; set; } = null!;
    public List<int> Pages { get; set; } = [];
}
