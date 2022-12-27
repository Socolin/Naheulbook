using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class InvalidItemOwnerTypeException : Exception
{
    public int ItemId { get; }

    public InvalidItemOwnerTypeException(int itemId)
    {
        ItemId = itemId;
    }
}