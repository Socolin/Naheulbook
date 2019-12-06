using System;
using System.Collections.Generic;
using Jose;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Configurations;
using Naheulbook.Web.Models;

namespace Naheulbook.Web.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(int userId);

        JwtTokenPayload? DecodeJwt(string jwt);
    }

    public class JwtService : IJwtService
    {
        private readonly IAuthenticationConfiguration _configuration;
        private readonly ITimeService _timeService;

        public JwtService(IAuthenticationConfiguration configuration, ITimeService timeService)
        {
            _configuration = configuration;
            _timeService = timeService;
        }

        public string GenerateJwtToken(int userId)
        {
            var expiration = _timeService.UtcNow.AddMinutes(_configuration.JwtExpirationDelayInMinutes).ToUnixTimeSeconds();

            var payload = new Dictionary<string, object>()
            {
                {"sub", userId},
                {"exp", expiration}
            };

            return JWT.Encode(payload, Convert.FromBase64String(_configuration.JwtSigningKey), JwsAlgorithm.HS256);
        }

        public JwtTokenPayload? DecodeJwt(string jwt)
        {
            try
            {
                return JWT.Decode<JwtTokenPayload>(jwt, Convert.FromBase64String(_configuration.JwtSigningKey));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}