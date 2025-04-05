

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Item;

public class InvalidItemOwnerTypeException(int itemId) : Exception
{
    public int ItemId { get; } = itemId;
}