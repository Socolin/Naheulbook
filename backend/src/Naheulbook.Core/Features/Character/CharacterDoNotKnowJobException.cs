// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Character;

public class CharacterDoNotKnowJobException(int characterId, Guid jobId) : Exception
{
    public int CharacterId { get; } = characterId;
    public Guid JobId { get; } = jobId;
}