namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateItemTemplateSubCategoryRequest
{
    public int SectionId { get; set; }
    public required string Name { get; set; }
    public string? TechName { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
}