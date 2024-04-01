using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Exceptions;

public class CharacterAlreadyKnowThisJobException(int characterId, Guid jobId) : Exception
{
    public int CharacterId { get; } = characterId;
    public Guid JobId { get; } = jobId;
}