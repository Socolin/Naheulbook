using System;
using System.Net;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.Google.Exceptions
{
    public class GoogleClientException : Exception
    {
        public string Content { get; }
        public HttpStatusCode StatusCode { get; }

        public GoogleClientException(string content, HttpStatusCode statusCode)
        {
            Content = content;
            StatusCode = statusCode;
        }
    }
}