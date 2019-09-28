using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Web.Responses
{
    public class StatModifierResponse
    {
        public string Stat { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Value { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string>? Special { get; set; }
    }
}