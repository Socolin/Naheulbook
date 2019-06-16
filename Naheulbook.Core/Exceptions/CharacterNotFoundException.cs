using System;

namespace Naheulbook.Core.Exceptions
{
    public class CharacterNotFoundException : Exception
    {
        public int CharacterId { get; }

        public CharacterNotFoundException(int groupId)
        {
            CharacterId = groupId;
        }
    }
}