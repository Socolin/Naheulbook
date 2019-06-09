using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses
{
    public class MonsterTemplateResponse
    {
        public class MonsterSimpleInventoryResponse
        {
            public int Id;
            public ItemTemplateResponse ItemTemplate;
            public int MinCount;
            public int MaxCount;
            public float Chance;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public List<int> Locations { get; set; }
        public JObject Data { get; set; }
        public List<MonsterSimpleInventoryResponse> SimpleInventory { get; set; }
    }
}