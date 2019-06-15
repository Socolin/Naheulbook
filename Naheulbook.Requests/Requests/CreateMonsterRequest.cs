using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Requests.Requests
{
    public class CreateMonsterRequest
    {
        public string Name { get; set; }
        public JObject Data { get; set; }
        public IList<ActiveStatsModifier> Modifiers { get; set; }
        public IList<object> Items { get; set; }
    }
}