using System;
using System.Collections.Generic;
using Jose;
using Naheulbook.Data.Models;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Configurations;

namespace Naheulbook.Web.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
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

        public string GenerateJwtToken(User user)
        {
            var expiration = _timeService.UtcNow.AddMinutes(_configuration.JwtExpirationDelayInMinutes).ToUnixTimeSeconds();

            var payload = new Dictionary<string, object>()
            {
                {"sub", user.Id},
                {"exp", expiration}
            };

            return JWT.Encode(payload, Convert.FromBase64String(_configuration.JwtSigningKey), JwsAlgorithm.HS256);
        }
    }
}