using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class ListActiveCharacterResponse
{
    public int Id { get; set; }
    public bool IsNpc { get; set; }
    public string Name { get; set; } = null!;
}