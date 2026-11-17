using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class GroupSummaryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CharacterCount { get; set; }
}