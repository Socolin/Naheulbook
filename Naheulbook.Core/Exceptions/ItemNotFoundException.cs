using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class ItemNotFoundException : Exception
{
    public int ItemId { get; }

    public ItemNotFoundException(int itemId)
    {
        ItemId = itemId;
    }
}