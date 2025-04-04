using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateLootRequest
{
    [StringLength(255, MinimumLength = 1)]
    public required string Name { get; set; }
}