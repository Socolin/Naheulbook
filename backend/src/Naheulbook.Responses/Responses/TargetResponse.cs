using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class TargetResponse
{
    public int Id { get; set; }
    public bool IsMonster { get; set; }
}