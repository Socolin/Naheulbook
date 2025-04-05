using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class CharacterRemoveJobResponse
{
    public Guid JobId { get; set; }
}