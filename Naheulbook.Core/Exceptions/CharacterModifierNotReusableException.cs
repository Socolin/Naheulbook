using System;

namespace Naheulbook.Core.Exceptions
{
    public class CharacterModifierNotReusableException : Exception
    {
        public int CharacterModifierId { get; }

        public CharacterModifierNotReusableException(int characterModifierId)
        {
            CharacterModifierId = characterModifierId;
        }
    }
}