// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Requests.Requests
{
    public class CreateItemTemplateCategoryRequest
    {
        public int SectionId { get; set; }
        public string Name { get; set; } = null!;
        public string TechName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Note { get; set; } = null!;
    }
}