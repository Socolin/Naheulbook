namespace Naheulbook.Data.Models
{
    public class ItemTemplateUnSkill
    {
        public int ItemTemplateId { get; set; }
        public ItemTemplate ItemTemplate { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}