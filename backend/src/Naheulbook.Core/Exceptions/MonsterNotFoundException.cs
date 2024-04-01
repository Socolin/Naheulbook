using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MonsterNotFoundException(int monsterId) : Exception
{
    public int MonsterId { get; } = monsterId;
}