using System;

namespace Naheulbook.Core.Exceptions;

public class CharacterModifierNotFoundException : Exception
{
    public int CharacterModifierId { get; }

    public CharacterModifierNotFoundException(int characterModifierId)
    {
        CharacterModifierId = characterModifierId;
    }
}