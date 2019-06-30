using System;

namespace Naheulbook.Core.Exceptions
{
    public class CharacterAlreadyInAGroupException : Exception
    {
        public int CharacterId { get; }

        public CharacterAlreadyInAGroupException(int characterId)
        {
            CharacterId = characterId;
        }
    }
}