using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.Oauth1;

public class OAuthException(string requestUrl, int statusCode, string body) : Exception($"Unexpected response status code {statusCode} during OAuth request to {requestUrl}")
{
    public string RequestUrl { get; } = requestUrl;
    public int StatusCode { get; } = statusCode;
    public string Body { get; } = body;
}