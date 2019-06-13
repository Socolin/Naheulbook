using Newtonsoft.Json;

namespace Naheulbook.Web.Responses
{
    public class MonsterCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("typeid")]
        public int TypeId { get; set; }
    }
}