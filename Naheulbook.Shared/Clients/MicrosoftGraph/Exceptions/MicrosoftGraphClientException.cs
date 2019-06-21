using System;
using System.Net;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.MicrosoftGraph.Exceptions
{
    public class MicrosoftGraphClientException : Exception
    {
        public string Content { get; }
        public HttpStatusCode StatusCode { get; }

        public MicrosoftGraphClientException(string content, HttpStatusCode statusCode)
        {
            Content = content;
            StatusCode = statusCode;
        }
    }
}