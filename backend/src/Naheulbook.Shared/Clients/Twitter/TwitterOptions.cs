using System;
using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Shared.Clients.Twitter;

[Serializable]
public class TwitterOptions
{
    [Required]
    public required string AppId { get; set; }

    [Required]
    public required string AppSecret { get; set; }

    [Required]
    public required string Callback { get; set; }
}