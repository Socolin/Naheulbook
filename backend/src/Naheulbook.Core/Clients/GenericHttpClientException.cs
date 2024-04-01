using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Clients;

public class GenericHttpClientException(string requestUrl, int statusCode, string body) : Exception($"Unexpected response status code {statusCode} during request to {requestUrl}")
{
    public string RequestUrl { get; } = requestUrl;
    public int StatusCode { get; } = statusCode;
    public string Body { get; } = body;
}