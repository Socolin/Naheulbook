using Jose;
using Microsoft.Extensions.Options;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Configurations;
using Naheulbook.Web.Models;

namespace Naheulbook.Web.Services;

public interface IJwtService
{
    string GenerateJwtToken(int userId);

    JwtTokenPayload? DecodeJwt(string jwt);
}

public class JwtService(IOptions<AuthenticationOptions> configuration, ITimeService timeService)
    : IJwtService
{
    public string GenerateJwtToken(int userId)
    {
        var expiration = timeService.UtcNow.AddMinutes(configuration.Value.JwtExpirationDelayInMinutes).ToUnixTimeSeconds();

        var payload = new Dictionary<string, object>()
        {
            {"sub", userId},
            {"exp", expiration},
        };

        return JWT.Encode(payload, Convert.FromBase64String(configuration.Value.JwtSigningKey), JwsAlgorithm.HS256);
    }

    public JwtTokenPayload? DecodeJwt(string jwt)
    {
        try
        {
            return JWT.Decode<JwtTokenPayload>(jwt, Convert.FromBase64String(configuration.Value.JwtSigningKey));
        }
        catch (Exception)
        {
            return null;
        }
    }
}