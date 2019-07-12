using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Models
{
    public class ItemData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public JToken Icon { get; set; }
        public int? Charge { get; set; }
        public int? Ug { get; set; }

        [JsonProperty("equiped")]
        public int? Equipped { get; set; }

        public int? ReadCount { get; set; }
        public bool? NotIdentified { get; set; }
        public bool? IgnoreRestrictions { get; set; }
        public JToken Lifetime { get; set; }
    }
}