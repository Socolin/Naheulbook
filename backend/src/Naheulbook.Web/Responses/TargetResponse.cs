using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class TargetResponse
{
    public int Id { get; set; }
    public bool IsMonster { get; set; }
}