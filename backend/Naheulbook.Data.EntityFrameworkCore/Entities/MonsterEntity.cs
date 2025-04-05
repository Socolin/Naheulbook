using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MonsterEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Data { get; set; }
    public DateTimeOffset? Dead { get; set; }

    public string? Modifiers { get; set; }

    public int? FightId { get; set; }
    private FightEntity? _fight;
    public FightEntity? Fight { get => _fight.ThrowIfNotLoadedAndNotNull(LootId); set => _fight = value; }

    public int GroupId { get; set; }
    private GroupEntity? _group;
    public GroupEntity Group { get => _group.ThrowIfNotLoaded(); set => _group = value; }

    public int? LootId { get; set; }
    private LootEntity? _loot;
    public LootEntity? Loot { get => _loot.ThrowIfNotLoadedAndNotNull(LootId); set => _loot = value; }

    public int? TargetedCharacterId { get; set; }
    private CharacterEntity? _targetedCharacter;
    public CharacterEntity? TargetedCharacter { get => _targetedCharacter.ThrowIfNotLoadedAndNotNull(TargetedCharacterId); set => _targetedCharacter = value; }

    public int? TargetedMonsterId { get; set; }
    private MonsterEntity? _targetedMonster;
    public MonsterEntity? TargetedMonster { get => _targetedMonster.ThrowIfNotLoadedAndNotNull(TargetedMonsterId); set => _targetedMonster = value; }

    private ICollection<ItemEntity>? _items;
    public ICollection<ItemEntity> Items { get => _items.ThrowIfNotLoaded(); set => _items = value; }
}