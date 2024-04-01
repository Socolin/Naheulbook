using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.MicrosoftGraph.Exceptions;

public class MicrosoftGraphClientException(string content, int statusCode) : Exception
{
    public string Content { get; } = content;
    public int StatusCode { get; } = statusCode;
}