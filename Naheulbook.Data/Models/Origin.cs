using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Origin
    {
        public Origin()
        {
            Bonuses = new HashSet<OriginBonus>();
            Information = new HashSet<OriginInfo>();
            Requirements = new HashSet<OriginRequirement>();
            Restrictions = new HashSet<OriginRestrict>();
            Skills = new HashSet<OriginSkill>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PlayerDescription { get; set; }
        public string PlayerSummary { get; set; }
        public long? MaxLoad { get; set; }
        public short? MaxArmorPr { get; set; }
        public string Advantage { get; set; }
        public short? BaseEv { get; set; }
        public short? BaseEa { get; set; }
        public short? BonusAt { get; set; }
        public short? BonusPrd { get; set; }
        public int DiceEvLevelUp { get; set; }
        public string Size { get; set; }
        public string Flags { get; set; }
        public short? SpeedModifier { get; set; }

        public ICollection<OriginBonus> Bonuses { get; set; }
        public ICollection<OriginInfo> Information { get; set; }
        public ICollection<OriginRequirement> Requirements { get; set; }
        public ICollection<OriginRestrict> Restrictions { get; set; }
        public ICollection<OriginSkill> Skills { get; set; }
    }
}