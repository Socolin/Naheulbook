using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Web.Responses
{
    public class CharacterLevelUpResponse
    {
        public List<ActiveStatsModifier> NewModifiers { get; set; }
        public List<int> NewSkillIds { get; set; }
        public int NewLevel { get; set; }
        public List<SpecialityResponse> NewSpecialities { get; set; }
    }
}