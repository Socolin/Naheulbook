namespace Naheulbook.Data.Models
{
    public class JobSkill
    {
        public bool Default { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}