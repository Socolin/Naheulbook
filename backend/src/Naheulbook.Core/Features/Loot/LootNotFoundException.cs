

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Loot;

public class LootNotFoundException(int lootId) : Exception
{
    public int LootId { get; } = lootId;
}