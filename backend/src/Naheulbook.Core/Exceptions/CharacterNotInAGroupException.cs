using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class CharacterNotInAGroupException : Exception
{
    public int CharacterId { get; }

    public CharacterNotInAGroupException(int characterId)
    {
        CharacterId = characterId;
    }
}