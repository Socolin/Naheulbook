using System.Collections.Generic;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateItemTemplateSectionRequest
{
    public required string Name { get; set; }
    public string? Note { get; set; }
    public required string Icon { get; set; }
    public required List<string> Specials { get; set; }
}