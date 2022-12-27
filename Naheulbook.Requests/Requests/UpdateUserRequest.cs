// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Requests.Requests;

public class UpdateUserRequest
{
    public string? DisplayName { get; set; }
    public int? ShowInSearchFor { get; set; }
}