using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class LootNotFoundException : Exception
{
    public int LootId { get; }

    public LootNotFoundException(int lootId)
    {
        LootId = lootId;
    }
}