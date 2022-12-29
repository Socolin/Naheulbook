namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class ValidateUserRequest
{
    public string ActivationCode { get; set; } = null!;
}