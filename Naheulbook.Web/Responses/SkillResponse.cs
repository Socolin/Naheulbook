using System.Collections.Generic;

namespace Naheulbook.Web.Responses
{
    public class SkillResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PlayerDescription { get; set; }
        public string Require { get; set; }
        public string Resist { get; set; }
        public string Using { get; set; }
        public string Roleplay { get; set; }
        public string[] Stat;
        public short? Test { get; set; }
        public List<FlagResponse> Flags { get; set; }
        public List<SkillEffectResponse> Effects { get; set; }
    }

    public class SkillEffectResponse
    {
        public string Stat { get; set; }
        public int Value { get; set; }
        public string Type { get; set; }
    }
}