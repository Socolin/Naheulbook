namespace Naheulbook.Data.Models
{
    public class SkillEffect
    {
        public int SkillId { get; set; }
        public string StatName { get; set; }
        public int Value { get; set; }
        public Skill Skill { get; set; }
    }
}