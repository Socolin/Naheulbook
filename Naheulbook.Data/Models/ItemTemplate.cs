using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class ItemTemplate
    {
        public ItemTemplate()
        {
            Modifiers = new HashSet<ItemTemplateModifier>();
            Requirements = new HashSet<ItemTemplateRequirement>();
            SkillModifiers = new HashSet<ItemTemplateSkillModifier>();
            Slots = new HashSet<ItemTemplateSlot>();
            Skills = new HashSet<ItemTemplateSkill>();
            UnSkills = new HashSet<ItemTemplateUnSkill>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string CleanName { get; set; }
        public string TechName { get; set; }
        public string Source { get; set; }
        public string SourceUserNameCache { get; set; }
        public string Data { get; set; }

        public int CategoryId { get; set; }
        public ItemTemplateCategory Category { get; set; }

        public int? SourceUserId { get; set; }
        public User SourceUser { get; set; }

        public ICollection<ItemTemplateModifier> Modifiers { get; set; }
        public ICollection<ItemTemplateRequirement> Requirements { get; set; }
        public ICollection<ItemTemplateSkillModifier> SkillModifiers { get; set; }
        public ICollection<ItemTemplateSlot> Slots { get; set; }
        public ICollection<ItemTemplateSkill> Skills { get; set; }
        public ICollection<ItemTemplateUnSkill> UnSkills { get; set; }
    }
}