using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests
{
    public class CreateMonsterRequest
    {
        public string Name { get; set; } = null!;
        public MonsterData? Data { get; set; }
        public IList<ActiveStatsModifier>? Modifiers { get; set; }
        public IList<CreateItemRequest> Items { get; set; } = null!;
    }
}