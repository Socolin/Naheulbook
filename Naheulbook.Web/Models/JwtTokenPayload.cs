// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Models;

public class JwtTokenPayload
{
    public int Sub { get; set; }
    public long Exp { get; set; }
}