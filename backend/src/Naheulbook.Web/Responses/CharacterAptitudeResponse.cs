using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class CharacterAptitudeResponse
{
    public AptitudeResponse Aptitude { get; set; } = null!;
    public int Count { get; set; }
    public bool Active { get; set; }
}