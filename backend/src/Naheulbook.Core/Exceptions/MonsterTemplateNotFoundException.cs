using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MonsterTemplateNotFoundException(int monsterTemplateId) : Exception
{
    public int MonsterTemplateId { get; } = monsterTemplateId;
}