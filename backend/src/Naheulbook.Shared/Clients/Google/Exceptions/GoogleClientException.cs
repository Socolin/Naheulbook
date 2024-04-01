using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.Google.Exceptions;

public class GoogleClientException(string content, int statusCode) : Exception
{
    public string Content { get; } = content;
    public int StatusCode { get; } = statusCode;
}