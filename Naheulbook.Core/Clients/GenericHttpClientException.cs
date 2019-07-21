using System;
using System.Net;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Clients
{
    public class GenericHttpClientException : Exception
    {
        public string RequestUrl { get; }
        public HttpStatusCode StatusCode { get; }
        public string Body { get; }

        public GenericHttpClientException(string requestUrl, HttpStatusCode statusCode, string body)
            : base($"Unexpected response status code {statusCode} during request to {requestUrl}")
        {
            RequestUrl = requestUrl;
            StatusCode = statusCode;
            Body = body;
        }
    }
}