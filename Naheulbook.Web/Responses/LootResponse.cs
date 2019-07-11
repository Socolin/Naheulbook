using System.Collections.Generic;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses
{
    public class LootResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("visibleForPlayer")]
        public bool IsVisibleForPlayer { get; set; }

        public List<ItemResponse> Items { get; set; }
        public List<MonsterResponse> Monsters { get; set; }
    }
}