using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

public class OriginSkillEntity
{
    public bool Default { get; set; }

    public Guid OriginId { get; set; }
    private OriginEntity? _origin;
    public OriginEntity Origin { get => _origin.ThrowIfNotLoaded(); set => _origin = value; }

    public Guid SkillId { get; set; }
    private SkillEntity? _skill;
    public SkillEntity Skill { get => _skill.ThrowIfNotLoaded(); set => _skill = value; }
}