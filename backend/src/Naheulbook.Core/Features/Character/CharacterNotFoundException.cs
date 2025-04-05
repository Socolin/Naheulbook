

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Character;

public class CharacterNotFoundException(int characterId) : Exception
{
    public int CharacterId { get; } = characterId;
}