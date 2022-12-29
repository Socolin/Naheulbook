using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.Facebook.Exceptions;

public class FacebookClientException : Exception
{
    public string Content { get; }
    public int StatusCode { get; }

    public FacebookClientException(string content, int statusCode)
    {
        Content = content;
        StatusCode = statusCode;
    }
}