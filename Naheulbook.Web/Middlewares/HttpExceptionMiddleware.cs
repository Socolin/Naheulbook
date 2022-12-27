using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Naheulbook.Web.Exceptions;
using Newtonsoft.Json;

namespace Naheulbook.Web.Middlewares;

public class HttpExceptionMiddleware
{
    private static readonly string[] ExcludedExceptionFields = {"TargetSite", "StackTrace", "Message", "Data", "InnerException", "HelpLink", "Source", "HResult"};
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly bool _displayExceptionFields;

    public HttpExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger(nameof(HttpExceptionMiddleware));
        _displayExceptionFields = configuration.GetValue<bool>("DisplayExceptionFields");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // FIXME: logger context
            await _next(context);
        }
        catch (HttpErrorException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                ex.Message
            }));
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            _logger.LogError(ex, "An unexpected error occured: " + ex.Message);
            if (_displayExceptionFields)
            {
                foreach (var propertyInfo in ex.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(f => !ExcludedExceptionFields.Contains(f.Name)))
                {
                    _logger.LogError("ERROR_DETAIL:" + propertyInfo.Name + "=" + propertyInfo.GetValue(ex));
                }
            }

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                Message = $"An unexpected error occured, and was logged with reference id: {context.TraceIdentifier}"
            }));

        }
    }
}