using System;
using System.Net;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.Oauth1
{
    public class OAuthException : Exception
    {
        public string RequestUrl { get; }
        public HttpStatusCode StatusCode { get; }
        public string Body { get; }

        public OAuthException(string requestUrl, HttpStatusCode statusCode, string body)
            : base($"Unexpected response status code {statusCode} during OAuth request to {requestUrl}")
        {
            RequestUrl = requestUrl;
            StatusCode = statusCode;
            Body = body;
        }
    }
}