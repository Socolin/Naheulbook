using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class MonsterTemplateResponse
{
    public class MonsterTemplateInventoryElementResponse
    {
        public int Id;
        public ItemTemplateResponse ItemTemplate = null!;
        public int MinCount;
        public int MaxCount;
        public float Chance;
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int SubCategoryId { get; set; }
    public JObject Data { get; set; } = null!;
    public List<MonsterTemplateInventoryElementResponse> Inventory { get; set; } = null!;
}