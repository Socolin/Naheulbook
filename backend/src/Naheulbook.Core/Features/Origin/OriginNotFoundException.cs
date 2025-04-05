

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Origin;

public class OriginNotFoundException(Guid originId) : Exception
{
    public Guid OriginId { get; } = originId;
}