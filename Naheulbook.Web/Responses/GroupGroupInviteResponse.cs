using System.Collections.Generic;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses
{
    public class GroupGroupInviteResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("origin")]
        public string OriginName { get; set; }

        [JsonProperty("jobs")]
        public IList<string> JobNames { get; set; }

        public bool FromGroup { get; set; }
    }
}
