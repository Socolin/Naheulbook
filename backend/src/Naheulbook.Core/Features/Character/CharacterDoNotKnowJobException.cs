// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using System;

namespace Naheulbook.Core.Features.Character;

public class CharacterDoNotKnowJobException(int characterId, Guid jobId) : Exception
{
    public int CharacterId { get; } = characterId;
    public Guid JobId { get; } = jobId;
}