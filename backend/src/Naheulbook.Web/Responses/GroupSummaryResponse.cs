using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class GroupSummaryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CharacterCount { get; set; }
}