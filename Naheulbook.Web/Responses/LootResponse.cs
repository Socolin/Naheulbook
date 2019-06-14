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

        public List<object> Items { get; set; } = new List<object>();
        public List<object> Monsters { get; set; } = new List<object>();
    }
}