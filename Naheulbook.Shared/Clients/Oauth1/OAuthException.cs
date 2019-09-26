using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.Oauth1
{
    public class OAuthException : Exception
    {
        public string RequestUrl { get; }
        public int StatusCode { get; }
        public string Body { get; }

        public OAuthException(string requestUrl, int statusCode, string body)
            : base($"Unexpected response status code {statusCode} during OAuth request to {requestUrl}")
        {
            RequestUrl = requestUrl;
            StatusCode = statusCode;
            Body = body;
        }
    }
}