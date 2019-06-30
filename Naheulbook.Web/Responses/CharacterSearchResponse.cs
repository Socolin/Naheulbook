using Newtonsoft.Json;

namespace Naheulbook.Web.Responses
{
    public class CharacterSearchResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("origin")]
        public string OriginName { get; set; }

        [JsonProperty("owner")]
        public string OwnerName { get; set; }
    }
}