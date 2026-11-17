using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class CharacterAddAptitudeResponse
{
    public AptitudeResponse Aptitude { get; set; } = null!;
    public int Count { get; set; }
    public bool Active { get; set; }
}