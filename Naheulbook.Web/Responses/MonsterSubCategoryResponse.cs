using Newtonsoft.Json;

namespace Naheulbook.Web.Responses
{
    public class MonsterSubCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [JsonProperty("typeid")]
        public int TypeId { get; set; }
    }
}