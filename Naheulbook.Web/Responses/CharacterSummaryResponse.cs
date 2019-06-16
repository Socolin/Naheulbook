using System.Collections.Generic;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses
{
    public class CharacterSummaryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }

        [JsonProperty("origin")]
        public string OriginName { get; set; }

        [JsonProperty("jobs")]
        public IList<string> JobNames { get; set; }

        // TODO: Check if this field can be remove
        public int OriginId { get; set; }
    }
}