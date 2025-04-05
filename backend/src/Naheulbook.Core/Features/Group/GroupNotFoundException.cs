

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Group;

public class GroupNotFoundException(int groupId) : Exception
{
    public int GroupId { get; } = groupId;
}