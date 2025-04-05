

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Monster;

public class MonsterTemplateNotFoundException(int monsterTemplateId) : Exception
{
    public int MonsterTemplateId { get; } = monsterTemplateId;
}