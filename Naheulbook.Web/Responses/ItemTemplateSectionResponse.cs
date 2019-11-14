using System.Collections.Generic;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Web.Responses
{
    public class ItemTemplateSectionResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Note { get; set; } = null!;
        public List<string> Specials { get; set; } = null!;
        public List<ItemTemplateSubCategoryResponse> SubCategories { get; set; } = null!;
    }
}