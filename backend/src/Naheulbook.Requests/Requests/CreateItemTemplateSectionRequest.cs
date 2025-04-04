using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateItemTemplateSectionRequest
{
    [StringLength(255, MinimumLength = 1)]
    public required string Name { get; set; }
    public string? Note { get; set; }
    public required string Icon { get; set; }
    public required List<string> Specials { get; set; }
}