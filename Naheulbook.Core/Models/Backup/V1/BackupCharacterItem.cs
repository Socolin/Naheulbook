namespace Naheulbook.Core.Models.Backup.V1;

public class BackupCharacterItem
{
    public string Data { get; set; } = null!;
    public string? Modifiers { get; set; }
    public int? ContainerId { get; set; }

    public BackupItemTemplate ItemTemplate { get; set; } = null!;
}