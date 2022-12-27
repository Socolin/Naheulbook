using System;
using Microsoft.AspNetCore.Http;

namespace Naheulbook.Web.Exceptions;

public class HttpErrorException : Exception
{
    public int StatusCode { get; }

    public HttpErrorException(int statusCode, string message, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    public HttpErrorException(int statusCode, Exception innerException)
        : base(GetMessage(statusCode), innerException)
    {
        StatusCode = statusCode;
    }

    public HttpErrorException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    private static string GetMessage(int statusCode)
    {
        switch (statusCode)
        {
            case StatusCodes.Status403Forbidden:
                return "You are not allowed to access this resource";
            default:
                return "An error occured";
        }
    }
}