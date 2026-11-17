using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class CharacterAptitudeResponse
{
    public AptitudeResponse Aptitude { get; set; } = null!;
    public int Count { get; set; }
    public bool Active { get; set; }
}