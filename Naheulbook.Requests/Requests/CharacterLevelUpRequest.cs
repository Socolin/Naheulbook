using System.Collections.Generic;

namespace Naheulbook.Requests.Requests
{
    public class CharacterLevelUpRequest
    {
        public string EvOrEa { get; set; }
        public short EvOrEaValue { get; set; }
        public int TargetLevelUp { get; set; }
        public string StatToUp { get; set; }
        public int? SkillId { get; set; }
        public List<int> SpecialityIds { get; set; }
    }
}