namespace Naheulbook.Data.Models
{
    public class SkillEffect
    {
        public string StatName { get; set; } = null!;
        public int Value { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}