

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Monster;

public class MonsterNotFoundException(int monsterId) : Exception
{
    public int MonsterId { get; } = monsterId;
}