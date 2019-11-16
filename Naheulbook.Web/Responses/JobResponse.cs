using System;
using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses
{
    public class JobResponse
    {
        public int Id { get; set; }
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
}