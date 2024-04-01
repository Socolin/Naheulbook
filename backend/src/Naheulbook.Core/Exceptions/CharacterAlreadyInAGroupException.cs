using System;

namespace Naheulbook.Core.Exceptions;

public class CharacterAlreadyInAGroupException(int characterId) : Exception
{
    public int CharacterId { get; } = characterId;
}