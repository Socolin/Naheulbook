using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Features.Character;

public class CharacterNotInAGroupException(int characterId) : Exception
{
    public int CharacterId { get; } = characterId;
}