using System;
using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Web.Responses;

public class CharacterLevelUpResponse
{
    public List<ActiveStatsModifier> NewModifiers { get; set; } = null!;
    public List<Guid> NewSkillIds { get; set; } = null!;
    public int NewLevel { get; set; }
    public List<SpecialityResponse> NewSpecialities { get; set; } = null!;
}