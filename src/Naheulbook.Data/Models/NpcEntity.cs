using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

public class NpcEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Data { get; set; } = null!;

    public int GroupId { get; set; }
    private GroupEntity? _group;
    public GroupEntity Group { get => _group.ThrowIfNotLoaded(); set => _group = value; }
}