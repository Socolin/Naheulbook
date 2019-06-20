using System;
using System.Net;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.Facebook.Exceptions
{
    public class FacebookClientException : Exception
    {
        public string Content { get; }
        public HttpStatusCode StatusCode { get; }

        public FacebookClientException(string content, HttpStatusCode statusCode)
        {
            Content = content;
            StatusCode = statusCode;
        }
    }
}