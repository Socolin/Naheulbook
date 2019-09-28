using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses
{
    public class LocationMapResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public JObject Data { get; set; } = null!;
        public string File { get; set; } = null!;

        [JsonProperty("gm")]
        public bool IsGm { get; set; }
    }
}