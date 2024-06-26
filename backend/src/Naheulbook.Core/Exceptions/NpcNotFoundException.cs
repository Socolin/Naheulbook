using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class NpcNotFoundException(int npcId) : Exception
{
    public int NpcId { get; } = npcId;
}