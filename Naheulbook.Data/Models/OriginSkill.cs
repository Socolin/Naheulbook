namespace Naheulbook.Data.Models
{
    public class OriginSkill
    {
        public bool Default { get; set; }

        public int OriginId { get; set; }
        public Origin Origin { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}