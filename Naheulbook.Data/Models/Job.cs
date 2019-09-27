using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? InternalName { get; set; }
        public string? Information { get; set; }
        public string? PlayerDescription { get; set; }
        public string? PlayerSummary { get; set; }
        public string? Flags { get; set; }

        public int? MaxLoad { get; set; }
        public short? MaxArmorPr { get; set; }
        public bool? IsMagic { get; set; }
        public short? BaseEv { get; set; }
        public double? FactorEv { get; set; }
        public short? BonusEv { get; set; }
        public short? BaseEa { get; set; }
        public short? DiceEaLevelUp { get; set; }
        public short? BaseAt { get; set; }
        public short? BasePrd { get; set; }

        public int? ParentJobId { get; set; }
        public Job? ParentJob { get; set; }

        public ICollection<JobBonus> Bonuses { get; set; } = null!;
        public ICollection<JobOriginBlacklist> OriginBlacklist { get; set; } = null!;
        public ICollection<JobOriginWhitelist> OriginWhitelist { get; set; } = null!;
        public ICollection<JobRequirement> Requirements { get; set; } = null!;
        public ICollection<JobRestrict> Restrictions { get; set; } = null!;
        public ICollection<JobSkill> Skills { get; set; } = null!;
        public ICollection<Speciality> Specialities { get; set; } = null!;
    }
}