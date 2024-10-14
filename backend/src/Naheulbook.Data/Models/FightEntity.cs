using System.Collections.Generic;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

public class FightEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public int GroupId { get; set; }
    private GroupEntity? _group;
    public GroupEntity Group { get => _group.ThrowIfNotLoaded(); set => _group = value; }

    private ICollection<MonsterEntity>? _monsters;
    public ICollection<MonsterEntity> Monsters { get => _monsters.ThrowIfNotLoaded(); set => _monsters = value; }
}