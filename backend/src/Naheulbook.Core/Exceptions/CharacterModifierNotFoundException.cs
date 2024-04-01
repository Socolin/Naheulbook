using System;

namespace Naheulbook.Core.Exceptions;

public class CharacterModifierNotFoundException(int characterModifierId) : Exception
{
    public int CharacterModifierId { get; } = characterModifierId;
}