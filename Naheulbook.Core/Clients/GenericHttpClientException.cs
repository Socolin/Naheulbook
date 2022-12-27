using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Clients;

public class GenericHttpClientException : Exception
{
    public string RequestUrl { get; }
    public int StatusCode { get; }
    public string Body { get; }

    public GenericHttpClientException(string requestUrl, int statusCode, string body)
        : base($"Unexpected response status code {statusCode} during request to {requestUrl}")
    {
        RequestUrl = requestUrl;
        StatusCode = statusCode;
        Body = body;
    }
}