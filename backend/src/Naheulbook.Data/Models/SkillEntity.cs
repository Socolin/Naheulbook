using System;
using System.Collections.Generic;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

public class SkillEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? PlayerDescription { get; set; }
    public string? Require { get; set; }
    public string? Resist { get; set; }
    public string? Using { get; set; }
    public string? Roleplay { get; set; }
    public string? Stat { get; set; }
    public short? Test { get; set; }
    public string? Flags { get; set; }

    private ICollection<SkillEffect>? _skillEffects;
    public ICollection<SkillEffect> SkillEffects { get => _skillEffects.ThrowIfNotLoaded(); set => _skillEffects = value; }

    private ICollection<OriginSkillEntity>? _originSkills;
    public ICollection<OriginSkillEntity> OriginSkills { get => _originSkills.ThrowIfNotLoaded(); set => _originSkills = value; }

    private ICollection<JobSkillEntity>? _jobSkills;
    public ICollection<JobSkillEntity> JobSkills { get => _jobSkills.ThrowIfNotLoaded(); set => _jobSkills = value; }
}