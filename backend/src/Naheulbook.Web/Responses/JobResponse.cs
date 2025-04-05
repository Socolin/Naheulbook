using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Web.Responses;


[PublicAPI]
public class JobResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Information { get; set; }

    public string? PlayerDescription { get; set; }
    public string? PlayerSummary { get; set; }

    public bool? IsMagic { get; set; }

    public JobData Data { get; set; } = null!;
    public List<FlagResponse>? Flags { get; set; }
    public List<Guid> SkillIds { get; set; } = null!;
    public List<Guid> AvailableSkillIds { get; set; } = null!;
    public List<DescribedFlagResponse> Bonuses { get; set; } = null!;
    public List<StatRequirementResponse> Requirements { get; set; } = null!;
    public List<DescribedFlagResponse> Restrictions { get; set; } = null!;
    public List<SpecialityResponse> Specialities { get; set; } = null!;
}