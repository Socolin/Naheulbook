using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class CharacterAddJobResponse
{
    public Guid JobId { get; set; }
}