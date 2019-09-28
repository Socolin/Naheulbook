using Naheulbook.Core.Models;
using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses
{
    public class CharacterFoGmResponse : CharacterResponse
    {
        [JsonProperty("active")]
        public bool IsActive { get; set; }

        public string Color { get; set; } = null!;
        public CharacterGmData? GmData { get; set; }
        public int? OwnerId { get; set; }
        public TargetResponse? Target { get; set; }
    }
}