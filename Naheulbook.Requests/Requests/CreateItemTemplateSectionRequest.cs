using System.Collections.Generic;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Requests.Requests;

public class CreateItemTemplateSectionRequest
{
    public string Name { get; set; } = null!;
    public string Note { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public List<string> Specials { get; set; } = new List<string>();
}