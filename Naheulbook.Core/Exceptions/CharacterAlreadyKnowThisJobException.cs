using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Exceptions
{
    public class CharacterAlreadyKnowThisJobException : Exception
    {
        public int CharacterId { get; }
        public Guid JobId { get; }

        public CharacterAlreadyKnowThisJobException(int characterId, Guid jobId)
        {
            CharacterId = characterId;
            JobId = jobId;
        }
    }
}