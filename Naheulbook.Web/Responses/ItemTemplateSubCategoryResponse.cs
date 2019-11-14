namespace Naheulbook.Web.Responses
{
    public class ItemTemplateSubCategoryResponse
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string TechName { get; set; } = null!;
        public string Note { get; set; } = null!;
    }
}