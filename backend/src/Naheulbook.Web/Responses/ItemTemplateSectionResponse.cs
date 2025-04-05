using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class ItemTemplateSectionResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Note { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public List<string> Specials { get; set; } = null!;
    public List<ItemTemplateSubCategoryResponse> SubCategories { get; set; } = null!;
}