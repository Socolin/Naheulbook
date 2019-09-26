using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Middlewares
{
    public class DevExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public DevExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger(nameof(DevExceptionMiddleware));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpErrorException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured: " + ex.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(ex.ToString());
            }
        }
    }
}