

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.Facebook.Exceptions;

public class FacebookClientException(string content, int statusCode) : Exception
{
    public string Content { get; } = content;
    public int StatusCode { get; } = statusCode;
}