using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class InviteNotFoundException : Exception
    {
        public int GroupId { get; }
        public int CharacterId { get; }

        public InviteNotFoundException(int characterId, int groupId)
        {
            CharacterId = characterId;
            GroupId = groupId;
        }
    }
}