using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Shared.Clients.MicrosoftGraph;

[Serializable]
public class MicrosoftGraphOptions
{
    [Required]
    public required string AppId { get; set; }

    [Required]
    public required string AppSecret { get; set; }
}