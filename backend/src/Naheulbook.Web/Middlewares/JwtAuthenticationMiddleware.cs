using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Features.Users;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Services;
using Newtonsoft.Json;

namespace Naheulbook.Web.Middlewares;

public class JwtAuthenticationMiddleware(
    RequestDelegate next,
    IJwtService jwtService,
    ITimeService timeService,
    IUserAccessTokenService userAccessTokenService
)
{
    private const string UserAccessTokenPrefix = "userAccessToken:";

    public async Task InvokeAsync(HttpContext context)
    {
        string? jwt = null;
        if (context.Request.Query.ContainsKey("access_token"))
            jwt = context.Request.Query["access_token"];
        else if (context.Request.Headers.TryGetValue("Authorization", out var header))
        {
            var authHeader = AuthenticationHeaderValue.Parse(header.ToString());
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
            // A bit hacky because signalr does not support other scheme for now.
            if (jwt.StartsWith(UserAccessTokenPrefix))
            {
                var token = await userAccessTokenService.ValidateTokenAsync(jwt.Substring(UserAccessTokenPrefix.Length));
                if (token == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new {Message = "Invalid AccessToken"}));
                    return;
                }

                context.SetExecutionContext(new NaheulbookExecutionContext
                    {
                        UserId = token.UserId,
                    }
                );
            }
            else
            {
                var token = jwtService.DecodeJwt(jwt);
                if (token == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new {Message = "Invalid JWT"}));
                    return;
                }

                if (token.Exp < timeService.UtcNow.ToUnixTimeSeconds())
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new {Message = "Expired JWT"}));
                    return;
                }

                context.SetExecutionContext(new NaheulbookExecutionContext
                    {
                        UserId = token.Sub,
                    }
                );
            }
        }

        await next(context);
    }
}