using System.Collections.Generic;

namespace Naheulbook.Requests.Requests
{
    public class CharacterLevelUpRequest
    {
        public string EvOrEa { get; set; } = null!;
        public short EvOrEaValue { get; set; }
        public int TargetLevelUp { get; set; }
        public string StatToUp { get; set; } = null!;
        public int? SkillId { get; set; }
        public List<int> SpecialityIds { get; set; } = null!;
    }
}