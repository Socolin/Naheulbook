using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class NpcNotFoundException : Exception
{
    public int NpcId { get; }

    public NpcNotFoundException(int npcId)
    {
        NpcId = npcId;
    }
}