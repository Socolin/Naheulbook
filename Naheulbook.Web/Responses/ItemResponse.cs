using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses
{
    public class ItemResponse
    {
        public int Id { get; set; }
        public JObject Data { get; set; }
        public List<ActiveStatsModifier> Modifiers { get; set; }

        [JsonProperty("template")]
        public ItemTemplateResponse ItemTemplate { get; set; }
    }
}