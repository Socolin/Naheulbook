using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses
{
    public class LocationMapResponse
    {
        public int Id { get; set; }
        public JObject Data { get; set; }
        public string File { get; set; }

        [JsonProperty("gm")]
        public bool IsGm { get; set; }

        public string Name { get; set; }
    }
}