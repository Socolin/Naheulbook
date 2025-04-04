using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Features.Loot;

public class LootNotFoundException(int lootId) : Exception
{
    public int LootId { get; } = lootId;
}