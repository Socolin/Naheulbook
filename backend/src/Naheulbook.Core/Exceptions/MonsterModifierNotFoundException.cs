using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MonsterModifierNotFoundException(int characterModifierId) : Exception
{
    public int MonsterModifierId { get; } = characterModifierId;
}