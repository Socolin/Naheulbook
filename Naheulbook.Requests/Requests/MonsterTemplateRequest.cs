using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Requests.Requests
{
    public class MonsterTemplateRequest
    {
        public string Name { get; set; }
        public JObject Data { get; set; }
        public int CategoryId { get; set; }
        public IList<MonsterSimpleInventoryRequest> SimpleInventory { get; set; } = new List<MonsterSimpleInventoryRequest>();
        public IList<int> LocationIds { get; set; } = new List<int>();
    }
}