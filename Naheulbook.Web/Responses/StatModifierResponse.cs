using System.Collections.Generic;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses
{
    public class StatModifierResponse
    {
        public string Stat { get; set; }
        public string Type { get; set; }
        public int Value { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Special { get; set; }
    }
}