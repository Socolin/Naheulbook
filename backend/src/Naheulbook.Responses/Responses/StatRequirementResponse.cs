using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class StatRequirementResponse
{
    public string Stat { get; set; } = null!;
    public int? Min { get; set; }
    public int? Max { get; set; }
}