namespace Naheulbook.Requests.Requests
{
    public class CreateItemTemplateCategoryRequest
    {
        public int SectionId { get; set; }
        public string Name { get; set; }
        public string TechName { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
    }
}