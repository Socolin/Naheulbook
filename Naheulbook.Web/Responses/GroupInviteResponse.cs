using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses
{
    public class GroupInviteResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool FromGroup { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; } = null!;

        [JsonProperty("origin")]
        public string OriginName { get; set; } = null!;

        [JsonProperty("jobs")]
        public IList<string> JobNames { get; set; } = null!;
    }
}