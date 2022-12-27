using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MonsterNotFoundException : Exception
{
    public int MonsterId { get; }

    public MonsterNotFoundException(int monsterId)
    {
        MonsterId = monsterId;
    }
}