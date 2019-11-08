using System.Collections.Generic;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Web.Responses
{
    public class MonsterTemplateResponse
    {
        public class MonsterSimpleInventoryResponse
        {
            public int Id;
            public ItemTemplateResponse ItemTemplate = null!;
            public int MinCount;
            public int MaxCount;
            public float Chance;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public JObject Data { get; set; } = null!;
        public List<MonsterSimpleInventoryResponse> SimpleInventory { get; set; } = null!;
    }
}