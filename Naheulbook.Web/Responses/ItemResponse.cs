using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses
{
    public class ItemResponse
    {
        public int Id { get; set; }
        public JObject Data { get; set; }
        public JArray Modifiers { get; set; }

        [JsonProperty("template")]
        public ItemTemplateResponse ItemTemplate { get; set; }
    }
}