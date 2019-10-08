using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Exceptions
{
    public class CharacterAlreadyKnowThisJobException : Exception
    {
        public int CharacterId { get; }
        public int JobId { get; }

        public CharacterAlreadyKnowThisJobException(int characterId, int jobId)
        {
            CharacterId = characterId;
            JobId = jobId;
        }
    }
}