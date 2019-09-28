using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Web.Responses
{
    public class ItemPartialResponse
    {
        public int Id { get; set; }
        public JObject Data { get; set; } = null!;
        public List<ActiveStatsModifier> Modifiers { get; set; } = null!;
        public int? ContainerId { get; set; }
    }
}