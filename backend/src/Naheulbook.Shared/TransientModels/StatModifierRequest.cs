namespace Naheulbook.Shared.TransientModels;

[Serializable]
public class StatModifierRequest
{
    public required string Stat { get; set; }
    public required string Type { get; set; }
    public short Value { get; set; }
    public IList<string>? Special { get; set; }
}