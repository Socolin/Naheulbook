using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class MonsterTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public IList<MonsterSubCategoryResponse> SubCategories { get; set; } = null!;
}