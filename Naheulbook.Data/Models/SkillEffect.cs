namespace Naheulbook.Data.Models
{
    public class SkillEffect
    {
        public long SkillId { get; set; }
        public string StatName { get; set; }
        public long Value { get; set; }
        public Skill Skill { get; set; }
    }
}