

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Item;

public class ItemNotFoundException(int itemId) : Exception
{
    public int ItemId { get; } = itemId;
}