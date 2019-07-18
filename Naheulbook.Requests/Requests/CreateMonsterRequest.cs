using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests
{
    public class CreateMonsterRequest
    {
        public string Name { get; set; }
        public MonsterData Data { get; set; }
        public IList<ActiveStatsModifier> Modifiers { get; set; }
        public IList<object> Items { get; set; }
    }
}