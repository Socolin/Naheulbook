

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Group;

public class InviteNotFoundException(int characterId, int groupId) : Exception
{
    public int GroupId { get; } = groupId;
    public int CharacterId { get; } = characterId;
}