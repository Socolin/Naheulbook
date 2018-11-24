using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Skill
    {
        public Skill()
        {
            SkillEffects = new HashSet<SkillEffect>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PlayerDescription { get; set; }
        public string Require { get; set; }
        public string Resist { get; set; }
        public string Using { get; set; }
        public string Roleplay { get; set; }
        public string Stat { get; set; }
        public short? Test { get; set; }
        public string Flags { get; set; }

        public ICollection<SkillEffect> SkillEffects { get; set; }

        public ICollection<OriginSkill> OriginSkills { get; set; }
        public ICollection<JobSkill> JobSkills { get; set; }
    }
}