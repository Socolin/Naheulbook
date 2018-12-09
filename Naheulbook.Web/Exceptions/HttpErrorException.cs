using System;
using System.Net;

namespace Naheulbook.Web.Exceptions
{
    public class HttpErrorException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public HttpErrorException(HttpStatusCode statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        public HttpErrorException(HttpStatusCode statusCode, Exception innerException)
            : base(GetMessage(statusCode), innerException)
        {
            StatusCode = statusCode;
        }

        private static string GetMessage(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Forbidden:
                    return "You are not allowed to access this resource";
                default:
                    return "An error occured";
            }
        }
    }
}