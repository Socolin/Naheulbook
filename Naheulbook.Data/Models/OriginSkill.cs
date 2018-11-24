namespace Naheulbook.Data.Models
{
    public class OriginSkill
    {
        public bool Default { get; set; }

        public long OriginId { get; set; }
        public Origin Origin { get; set; }

        public long SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}