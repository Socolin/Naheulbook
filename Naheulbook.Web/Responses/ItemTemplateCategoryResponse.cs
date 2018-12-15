using System.Collections.Generic;

namespace Naheulbook.Web.Responses
{
    public class ItemTemplateCategoryResponse
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TechName { get; set; }
        public string Note { get; set; }
        public List<ItemTemplateResponse> ItemTemplates { get; set; }
    }
}