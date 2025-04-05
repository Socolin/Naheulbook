using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class DescribedFlagResponse
{
    public string Description { get; set; } = null!;
    public List<FlagResponse> Flags { get; set; } = null!;
}