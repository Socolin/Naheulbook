namespace Naheulbook.Core.Features.Entropy;

public class CharacterNotInGroupException(int groupId, int characterId) : Exception($"Character {characterId} is not in group {groupId}")
{
    public int GroupId { get; } = groupId;
    public int CharacterId { get; } = characterId;
}
