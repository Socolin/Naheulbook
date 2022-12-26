using Newtonsoft.Json.Linq;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Web.Responses
{
    public class DeadMonsterResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Dead { get; set; }
        public JObject? Data { get; set; }
    }
}