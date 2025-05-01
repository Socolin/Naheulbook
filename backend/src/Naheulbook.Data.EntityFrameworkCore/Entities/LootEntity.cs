using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class LootEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsVisibleForPlayer { get; set; }
    public DateTime? Created { get; set; }

    public int GroupId { get; set; }
    private GroupEntity? _group;
    public GroupEntity Group { get => _group.ThrowIfNotLoaded(); set => _group = value; }

    private ICollection<MonsterEntity>? _monsters;
    public ICollection<MonsterEntity> Monsters { get => _monsters.ThrowIfNotLoaded(); set => _monsters = value; }

    private ICollection<ItemEntity>? _items;
    public ICollection<ItemEntity> Items { get => _items.ThrowIfNotLoaded(); set => _items = value; }
}