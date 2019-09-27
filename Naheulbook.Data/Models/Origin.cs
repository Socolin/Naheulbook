using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Origin
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? PlayerDescription { get; set; }
        public string? PlayerSummary { get; set; }
        public int? MaxLoad { get; set; }
        public short? MaxArmorPr { get; set; }
        public string? Advantage { get; set; }
        public short? BaseEv { get; set; }
        public short? BaseEa { get; set; }
        public short? BonusAt { get; set; }
        public short? BonusPrd { get; set; }
        public int DiceEvLevelUp { get; set; }
        public string? Size { get; set; }
        public string? Flags { get; set; }
        public short? SpeedModifier { get; set; }

        public ICollection<OriginBonus> Bonuses { get; set; } = null!;
        public ICollection<OriginInfo> Information { get; set; } = null!;
        public ICollection<OriginRequirement> Requirements { get; set; } = null!;
        public ICollection<OriginRestrict> Restrictions { get; set; } = null!;
        public ICollection<OriginSkill> Skills { get; set; } = null!;
    }
}