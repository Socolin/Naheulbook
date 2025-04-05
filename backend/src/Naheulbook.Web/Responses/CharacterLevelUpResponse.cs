using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class CharacterLevelUpResponse
{
    public List<ActiveStatsModifier> NewModifiers { get; set; } = null!;
    public List<Guid> NewSkillIds { get; set; } = null!;
    public int NewLevel { get; set; }
    public List<SpecialityResponse> NewSpecialities { get; set; } = null!;
}