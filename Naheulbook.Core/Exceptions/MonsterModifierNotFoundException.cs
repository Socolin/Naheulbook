using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class MonsterModifierNotFoundException : Exception
    {
        public int MonsterModifierId { get; }

        public MonsterModifierNotFoundException(int characterModifierId)
        {
            MonsterModifierId = characterModifierId;
        }
    }
}