using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.MicrosoftGraph.Exceptions;

public class MicrosoftGraphClientException : Exception
{
    public string Content { get; }
    public int StatusCode { get; }

    public MicrosoftGraphClientException(string content, int statusCode)
    {
        Content = content;
        StatusCode = statusCode;
    }
}