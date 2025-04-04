using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Features.Item;

public class ItemNotFoundException(int itemId) : Exception
{
    public int ItemId { get; } = itemId;
}