using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateItemTemplateSubCategoryRequest
{
    public int SectionId { get; set; }

    [StringLength(255, MinimumLength = 1)]
    public required string Name { get; set; }

    [StringLength(255)]
    public string? TechName { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public string? Note { get; set; }
}