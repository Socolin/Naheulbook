using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.Google.Exceptions
{
    public class GoogleClientException : Exception
    {
        public string Content { get; }
        public int StatusCode { get; }

        public GoogleClientException(string content, int statusCode)
        {
            Content = content;
            StatusCode = statusCode;
        }
    }
}