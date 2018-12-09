using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Naheulbook.Web.Exceptions;
using Newtonsoft.Json;

namespace Naheulbook.Web.Middlewares
{
    public class HttpExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
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
                context.Response.StatusCode = (int) ex.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    ex.Message
                }));
            }
            catch (Exception)
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                // FIXME log
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Message = $"An unexpected error occured, and was logged with reference id: {context.TraceIdentifier}"
                }));
            }
        }
    }
}