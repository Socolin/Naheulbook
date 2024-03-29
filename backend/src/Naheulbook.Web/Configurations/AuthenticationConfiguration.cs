namespace Naheulbook.Web.Configurations;

public interface IAuthenticationConfiguration
{
    string JwtSigningKey { get; }
    int JwtExpirationDelayInMinutes { get; }
}

public class AuthenticationConfiguration : IAuthenticationConfiguration
{
    public string JwtSigningKey { get; set; } = null!;
    public int JwtExpirationDelayInMinutes { get; set; } = 20;
}