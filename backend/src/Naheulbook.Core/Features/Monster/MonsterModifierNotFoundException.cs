

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Monster;

public class MonsterModifierNotFoundException(int characterModifierId) : Exception
{
    public int MonsterModifierId { get; } = characterModifierId;
}