using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class GroupHistoryEntryResponse : IHistoryEntryResponse
{
    public int Id { get; set; }
    public string Action { get; set; } = null!;
    public JObject? Data { get; set; }
    public string Date { get; set; } = null!;
    public bool Gm { get; set; }
    public string? Info { get; set; }
    public bool IsGroup => true;
}