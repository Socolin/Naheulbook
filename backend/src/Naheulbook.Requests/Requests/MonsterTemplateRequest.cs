using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class MonsterTemplateRequest
{
    [StringLength(255, MinimumLength = 1)]
    public required string Name { get; set; }

    public required JObject Data { get; set; }
    public int SubCategoryId { get; set; }
    public IList<MonsterTemplateInventoryElementRequest> Inventory { get; set; } = new List<MonsterTemplateInventoryElementRequest>();
    public IList<int> LocationIds { get; set; } = new List<int>();
}