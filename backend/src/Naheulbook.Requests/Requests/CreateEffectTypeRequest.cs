using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateEffectTypeRequest
{
    [StringLength(255, MinimumLength = 1)]
    public required string Name { get; set; }
}