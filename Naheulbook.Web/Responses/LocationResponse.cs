using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses
{
    public class LocationResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [JsonProperty("parent")]
        public int? ParentId { get; set; }

        public JObject? Data { get; set; }
    }
}