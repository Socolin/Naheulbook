using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Shared.TransientModels
{
    public class PartialItemTemplateData
    {
        public int? Charge { get; set; }
        public JObject? Icon { get; set; }
        public JObject? Lifetime { get; set; }
        public string? NotIdentifiedName { get; set; }
        public bool? Quantifiable { get; set; }
        [JsonProperty("UseUG")]
        public bool? UseUg { get; set; }
    }
}