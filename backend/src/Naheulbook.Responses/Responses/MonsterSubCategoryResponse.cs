using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class MonsterSubCategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    [JsonProperty("typeid")]
    public int TypeId { get; set; }
}