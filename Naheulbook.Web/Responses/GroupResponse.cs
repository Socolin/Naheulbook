using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses
{
    public class GroupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public JObject Data { get; set; }
        public LocationResponse Location { get; set; }

        public IList<int> CharacterIds { get; set; }
        public IList<GroupGroupInviteResponse> Invites { get; set; }
    }
}