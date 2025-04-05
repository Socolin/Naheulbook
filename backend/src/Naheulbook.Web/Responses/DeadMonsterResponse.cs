using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class DeadMonsterResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Dead { get; set; }
    public JObject? Data { get; set; }
}