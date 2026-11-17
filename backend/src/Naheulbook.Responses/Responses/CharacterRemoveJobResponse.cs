using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class CharacterRemoveJobResponse
{
    public Guid JobId { get; set; }
}