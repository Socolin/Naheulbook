namespace Naheulbook.Core.Features.Character;

public class CharacterModifierNotReusableException(int characterModifierId) : Exception
{
    public int CharacterModifierId { get; } = characterModifierId;
}