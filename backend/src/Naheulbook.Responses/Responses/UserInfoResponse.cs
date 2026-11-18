using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class UserInfoResponse
{
    public int Id { get; set; }
    public string? DisplayName { get; set; }
    public bool ShowInSearch { get; set; }
    public bool Admin { get; set; }
    public bool LinkedWithFb { get; set; }
    public bool LinkedWithGoogle { get; set; }
    public bool LinkedWithMicrosoft { get; set; }
}