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

        public IList<object> Invites { get; set; } = new List<object>();
        public IList<object> Invited { get; set; } = new List<object>();
        public IList<object> Characters { get; set; } = new List<object>();
    }
}