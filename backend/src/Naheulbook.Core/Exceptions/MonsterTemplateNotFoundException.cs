using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MonsterTemplateNotFoundException : Exception
{
    public int MonsterTemplateId { get; }

    public MonsterTemplateNotFoundException(int monsterTemplateId)
    {
        MonsterTemplateId = monsterTemplateId;
    }
}