using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class StatRequirementResponse
{
    public string Stat { get; set; } = null!;
    public int? Min { get; set; }
    public int? Max { get; set; }
}