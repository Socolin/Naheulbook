using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class CharacterAddJobResponse
{
    public Guid JobId { get; set; }
}