namespace Naheulbook.Data.Models
{
    public class JobSkill
    {
        public bool Default { get; set; }

        public long JobId { get; set; }
        public virtual Job Job { get; set; }

        public long SkillId { get; set; }
        public virtual Skill Skill { get; set; }
    }
}