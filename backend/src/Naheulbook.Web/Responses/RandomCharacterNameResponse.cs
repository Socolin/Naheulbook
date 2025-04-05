using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class RandomCharacterNameResponse
{
    public string Name { get; set; } = null!;
}