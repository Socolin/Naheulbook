using System;

namespace Naheulbook.Core.Exceptions;

public class CharacterModifierNotReusableException(int characterModifierId) : Exception
{
    public int CharacterModifierId { get; } = characterModifierId;
}