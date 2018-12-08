using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Naheulbook.Core.Models;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Filters
{
    public class JwtAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IJwtService _jwtService;

        public JwtAuthorizationFilter(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var authHeader = AuthenticationHeaderValue.Parse(context.HttpContext.Request.Headers["Authorization"]);
            if (authHeader.Scheme != "JWT")
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userId = _jwtService.DecodeJwt(authHeader.Parameter);
            if (userId == null)
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                context.HttpContext.SetExecutionContext(new NaheulbookExecutionContext
                {
                    UserId = userId.Value
                });
            }
        }
    }
}