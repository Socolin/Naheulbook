using System;
using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class Origin
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

        public ICollection<OriginBonus> Bonuses { get; set; } = null!;
        public ICollection<OriginInfo> Information { get; set; } = null!;
        public ICollection<OriginRequirement> Requirements { get; set; } = null!;
        public ICollection<OriginRestrict> Restrictions { get; set; } = null!;
        public ICollection<OriginSkill> Skills { get; set; } = null!;
    }
}