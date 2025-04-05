using JetBrains.Annotations;

namespace Naheulbook.Web.Models;

[PublicAPI]
public class JwtTokenPayload
{
    public int Sub { get; set; }
    public long Exp { get; set; }
}