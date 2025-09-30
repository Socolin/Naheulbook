using System.Runtime.InteropServices;
using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class OriginEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Data { get; set; } = null!;
    public string? PlayerDescription { get; set; }
    public string? PlayerSummary { get; set; }
    public string? Advantage { get; set; }
    public string? Size { get; set; }
    public string? Flags { get; set; }

    public Guid AptitudeGroupId { get; set; }
    private AptitudeGroupEntity? _aptitudeGroup;
    public AptitudeGroupEntity AptitudeGroup { get => _aptitudeGroup.ThrowIfNotLoaded(); set => _aptitudeGroup = value; }

    private ICollection<OriginBonusEntity>? _bonuses;
    public ICollection<OriginBonusEntity> Bonuses { get => _bonuses.ThrowIfNotLoaded(); set => _bonuses = value; }

    private ICollection<OriginInfoEntity>? _information;
    public ICollection<OriginInfoEntity> Information { get => _information.ThrowIfNotLoaded(); set => _information = value; }

    private ICollection<OriginRequirementEntity>? _requirements;
    public ICollection<OriginRequirementEntity> Requirements { get => _requirements.ThrowIfNotLoaded(); set => _requirements = value; }

    private ICollection<OriginRestrictEntity>? _restrictions;
    public ICollection<OriginRestrictEntity> Restrictions { get => _restrictions.ThrowIfNotLoaded(); set => _restrictions = value; }

    private ICollection<OriginSkillEntity>? _skills;
    public ICollection<OriginSkillEntity> Skills { get => _skills.ThrowIfNotLoaded(); set => _skills = value; }
}