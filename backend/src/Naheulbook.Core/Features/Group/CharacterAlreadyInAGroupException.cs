namespace Naheulbook.Core.Features.Group;

[Serializable]
public class CharacterAlreadyInAGroupException(int characterId) : Exception
{
    public int CharacterId { get; } = characterId;
}