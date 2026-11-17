using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class DescribedFlagResponse
{
    public string Description { get; set; } = null!;
    public List<FlagResponse> Flags { get; set; } = null!;
}