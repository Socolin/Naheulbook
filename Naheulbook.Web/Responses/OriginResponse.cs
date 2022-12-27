using System;
using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses;

public class OriginResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? PlayerDescription { get; set; }
    public string? PlayerSummary { get; set; }
    public string? Advantage { get; set; }
    public OriginData Data { get; set; } = null!;
    public string? Size { get; set; }
    public List<FlagResponse>? Flags { get; set; }
    public List<Guid> SkillIds { get; set; } = null!;
    public List<Guid> AvailableSkillIds { get; set; } = null!;
    public IEnumerable<OriginInformationResponse> Information { get; set; } = null!;
    public ICollection<DescribedFlagResponse> Bonuses { get; set; } = null!;
    public ICollection<OriginRequirementResponse> Requirements { get; set; } = null!;
    public ICollection<DescribedFlagResponse> Restrictions { get; set; } = null!;
}

public class OriginRequirementResponse
{
    public string Stat { get; set; } = null!;
    public int? Min { get; set; }
    public int? Max { get; set; }
}

public class OriginInformationResponse
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
}