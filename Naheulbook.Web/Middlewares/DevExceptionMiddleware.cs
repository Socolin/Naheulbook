using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Naheulbook.Core.Exceptions;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Middlewares
{
    public class DevExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly bool _displayExceptionFields;

        public DevExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger(nameof(DevExceptionMiddleware));
            _displayExceptionFields = configuration.GetValue<bool>("DisplayExceptionFields");
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

                if (_displayExceptionFields)
                {
                    var excludedNames = new[] {"TargetSite", "StackTrace", "Message", "Data", "InnerException", "HelpLink", "Source", "HResult"};
                    foreach (var propertyInfo in ex.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(f => !excludedNames.Contains(f.Name)))
                    {
                        await context.Response.WriteAsync("\n" + propertyInfo.Name + "=" + propertyInfo.GetValue(ex));
                    }
                }
            }
        }
    }
}