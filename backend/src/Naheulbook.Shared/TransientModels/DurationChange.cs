namespace Naheulbook.Shared.TransientModels;

[Serializable]
public abstract class FighterDurationChanges
{
    public IList<IDurationChange> Changes { get; set; } = null!;
}

[Serializable]
public class CharacterUpdateDuration : FighterDurationChanges
{
    public int CharacterId { get; set; }
}

[Serializable]
public class MonsterUpdateDuration : FighterDurationChanges
{
    public int MonsterId { get; set; }
}

public interface IDurationChange
{
    string Type { get; }
}

public interface IITemDurationChange : IDurationChange
{
    int ItemId { get; set; }
}

public interface IModifierChange : IDurationChange
{
    NewModifierDurationValue Modifier { get; set; }
}

[Serializable]
public class ItemModifierDurationChange : IITemDurationChange, IModifierChange
{
    public const string TypeValue = "itemModifier";
    public string Type => TypeValue;
    public int ItemId { get; set; }
    public int ModifierIdx { get; set; }
    public NewModifierDurationValue Modifier { get; set; } = null!;
}

[Serializable]
public class ItemLifetimeDurationChange : IITemDurationChange
{
    public const string TypeValue = "itemLifetime";
    public string Type => TypeValue;
    public int ItemId { get; set; }
    public LifeTime LifeTime { get; set; } = null!;
}

[Serializable]
public class ModifierDurationChange : IModifierChange
{
    public const string TypeValue = "modifier";
    public string Type => TypeValue;
    public NewModifierDurationValue Modifier { get; set; } = null!;
}

[Serializable]
public class NewModifierDurationValue
{
    public int Id { get; set; }
    public bool Active { get; set; }

    public int? CurrentCombatCount { get; set; }
    public int? CurrentLapCount { get; set; }
    public int? CurrentTimeDuration { get; set; }
}