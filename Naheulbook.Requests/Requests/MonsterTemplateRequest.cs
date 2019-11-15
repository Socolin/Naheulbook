using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Requests.Requests
{
    public class MonsterTemplateRequest
    {
        public string Name { get; set; } = null!;
        public JObject Data { get; set; } = null!;
        public int SubCategoryId { get; set; }
        public IList<MonsterTemplateInventoryElementRequest> SimpleInventory { get; set; } = new List<MonsterTemplateInventoryElementRequest>();
        public IList<int> LocationIds { get; set; } = new List<int>();
    }
}