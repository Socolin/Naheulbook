

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Shared.Clients.Twitter.Exceptions;

public class TwitterClientException(Exception innerException) : Exception($"Error while requesting twitter api: {innerException.Message}", innerException);