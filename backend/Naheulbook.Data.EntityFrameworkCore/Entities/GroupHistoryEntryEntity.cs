using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class GroupHistoryEntryEntity : IHistoryEntry
{
    public int Id { get; set; }
    public string Action { get; set; } = null!;
    public string? Data { get; set; }
    public DateTime Date { get; set; }
    public bool Gm { get; set; }
    public string? Info { get; set; }

    public int GroupId { get; set; }
    private GroupEntity? _group;
    public GroupEntity Group { get => _group.ThrowIfNotLoaded(); set => _group = value; }
}