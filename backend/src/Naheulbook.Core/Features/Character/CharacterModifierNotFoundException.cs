namespace Naheulbook.Core.Features.Character;

public class CharacterModifierNotFoundException(int characterModifierId) : Exception
{
    public int CharacterModifierId { get; } = characterModifierId;
}