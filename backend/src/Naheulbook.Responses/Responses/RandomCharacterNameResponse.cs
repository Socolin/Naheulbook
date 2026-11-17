using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class RandomCharacterNameResponse
{
    public string Name { get; set; } = null!;
}