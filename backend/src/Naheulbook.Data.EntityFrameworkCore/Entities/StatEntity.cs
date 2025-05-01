using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class StatEntity
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Bonus { get; set; }
    public string? Penalty { get; set; }

    private IEnumerable<JobRequirementEntity>? _jobRequirements;
    public IEnumerable<JobRequirementEntity> JobRequirements { get => _jobRequirements.ThrowIfNotLoaded(); set => _jobRequirements = value; }

    private IEnumerable<EffectModifierEntity>? _effects;
    public IEnumerable<EffectModifierEntity> Effects { get => _effects.ThrowIfNotLoaded(); set => _effects = value; }
}