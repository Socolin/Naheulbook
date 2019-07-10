using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class MonsterModifierNotReusableException : Exception
    {
        public int MonsterModifierId { get; }

        public MonsterModifierNotReusableException(int characterModifierId)
        {
            MonsterModifierId = characterModifierId;
        }
    }
}