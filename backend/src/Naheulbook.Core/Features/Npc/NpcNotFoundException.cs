

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Npc;

public class NpcNotFoundException(int npcId) : Exception
{
    public int NpcId { get; } = npcId;
}