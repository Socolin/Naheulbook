using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class InviteNotFoundException(int characterId, int groupId) : Exception
{
    public int GroupId { get; } = groupId;
    public int CharacterId { get; } = characterId;
}