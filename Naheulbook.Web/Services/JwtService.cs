using System;
using System.Collections.Generic;
using Jose;
using Naheulbook.Data.Models;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Configurations;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(int userId);

        int? DecodeJwt(string jwt);
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

        public int? DecodeJwt(string jwt)
        {
            try
            {
                var result = JWT.Decode<JObject>(jwt, Convert.FromBase64String(_configuration.JwtSigningKey));
                return result.Value<int>("sub");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}