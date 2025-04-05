using System;

namespace Naheulbook.Web.Configurations;

[Serializable]
public class AuthenticationOptions
{
    public required string JwtSigningKey { get; set; }
    public int JwtExpirationDelayInMinutes { get; set; } = 20;
}