using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class CharacterAddAptitudeResponse
{
    public AptitudeResponse Aptitude { get; set; } = null!;
    public int Count { get; set; }
    public bool Active { get; set; }
}