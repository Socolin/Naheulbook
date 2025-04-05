using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class FlagResponse
{
    public string Type { get; set; } = null!;
    public JToken? Data { get; set; }
}