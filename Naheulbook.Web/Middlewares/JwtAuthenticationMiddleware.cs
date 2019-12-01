using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Naheulbook.Core.Models;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Services;
using Newtonsoft.Json;

namespace Naheulbook.Web.Middlewares
{
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtService _jwtService;
        private readonly ITimeService _timeService;

        public JwtAuthenticationMiddleware(RequestDelegate next, IJwtService jwtService, ITimeService timeService)
        {
            _next = next;
            _jwtService = jwtService;
            _timeService = timeService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? jwt = null;
            if (context.Request.Query.ContainsKey("access_token"))
                jwt = context.Request.Query["access_token"];
            else if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
                if (authHeader.Scheme != "JWT" && authHeader.Scheme != "Bearer")
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new {Message = "Invalid Authorization scheme"}));
                    return;
                }

                jwt = authHeader.Parameter;
            }

            if (jwt != null)
            {
                var token = _jwtService.DecodeJwt(jwt);
                if (token == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new {Message = "Invalid JWT"}));
                    return;
                }
                if (token.Exp < _timeService.UtcNow.ToUnixTimeSeconds())
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new {Message = "Expired JWT"}));
                    return;
                }

                context.SetExecutionContext(new NaheulbookExecutionContext
                {
                    UserId = token.Sub
                });
            }

            await _next(context);
        }
    }
}