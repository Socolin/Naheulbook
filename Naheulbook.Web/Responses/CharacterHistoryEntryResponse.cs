using Newtonsoft.Json.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Web.Responses
{
    public class CharacterHistoryEntryResponse : IHistoryEntryResponse
    {
        public int Id { get; set; }
        public string Action { get; set; } = null!;
        public JObject? Data { get; set; }
        public string Date { get; set; } = null!;
        public bool Gm { get; set; }
        public bool IsGroup => false;
    }
}