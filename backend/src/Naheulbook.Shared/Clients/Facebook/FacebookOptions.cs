using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Shared.Clients.Facebook;

[Serializable]
public class FacebookOptions
{
    [Required]
    public required string AppId { get; set; }

    [Required]
    public required string AppSecret { get; set; }
}