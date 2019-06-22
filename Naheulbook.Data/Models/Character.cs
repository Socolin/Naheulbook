using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Character
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Sex { get; set; }
        public bool IsActive { get; set; }
        public bool IsNpc { get; set; }
        public string Color { get; set; }

        public int Ad { get; set; }
        public int Cha { get; set; }
        public int Cou { get; set; }
        public int Fo { get; set; }
        public int Int { get; set; }

        public int? Ev { get; set; }
        public int? Ea { get; set; }

        public string GmData { get; set; }
        public short FatePoint { get; set; }
        public string StatBonusAd { get; set; }

        public int Level { get; set; }
        public int Experience { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; }

        public int OriginId { get; set; }
        public Origin Origin { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public int? TargetedCharacterId { get; set; }
        public Character TargetedCharacter { get; set; }

        public int? TargetedMonsterId { get; set; }
        public Monster TargetedMonster { get; set; }

        public ICollection<CharacterJob> Jobs { get; set; }
        public ICollection<CharacterModifier> Modifiers { get; set; }
        public ICollection<CharacterSkill> Skills { get; set; }
        public ICollection<CharacterSpeciality> Specialities { get; set; }
        public ICollection<Item> Items { get; set; }
        public ICollection<GroupInvite> Invites { get; set; }
    }
}