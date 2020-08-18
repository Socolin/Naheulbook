using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses
{
    public class GroupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public JObject? Data { get; set; }
        public GroupConfig Config { get; set; } = null!;

        public IList<int> CharacterIds { get; set; } = null!;
        public IList<GroupGroupInviteResponse> Invites { get; set; } = null!;
    }
}