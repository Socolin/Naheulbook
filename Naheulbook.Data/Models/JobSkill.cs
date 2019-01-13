namespace Naheulbook.Data.Models
{
    public class JobSkill
    {
        public bool Default { get; set; }

        public int JobId { get; set; }
        public virtual Job Job { get; set; }

        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; }
    }
}