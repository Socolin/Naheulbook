using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.Twitter.Exceptions
{
    public class TwitterClientException : Exception
    {
        public TwitterClientException(Exception innerException)
            : base($"Error while requesting twitter api: {innerException.Message}", innerException)
        {
        }
    }
}