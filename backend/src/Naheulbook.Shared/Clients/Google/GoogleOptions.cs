using System;
using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Shared.Clients.Google;

[Serializable]
public class GoogleOptions
{
    [Required]
    public required string AppId { get; set; }

    [Required]
    public required string AppSecret { get; set; }
}