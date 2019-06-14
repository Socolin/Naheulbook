using System.Collections.Generic;
using Naheulbook.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses
{
    public class LocationResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("parent")]
        public int ParentId { get; set; }

        [JsonProperty("sons")]
        public IList<Location> Children { get; set; }
        public JObject Data { get; set; }
    }
}