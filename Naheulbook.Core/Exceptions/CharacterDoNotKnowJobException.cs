// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

using System;

namespace Naheulbook.Core.Exceptions
{
    public class CharacterDoNotKnowJobException : Exception
    {
        public int CharacterId { get; }
        public Guid JobId { get; }

        public CharacterDoNotKnowJobException(int characterId, Guid jobId)
        {
            CharacterId = characterId;
            JobId = jobId;
        }
    }
}