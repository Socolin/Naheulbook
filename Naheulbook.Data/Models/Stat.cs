using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Stat
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Bonus { get; set; }
        public string Penalty { get; set; }

        public IEnumerable<JobRequirement> JobRequirements { get; set; }
        public IEnumerable<EffectModifier> Effects { get; set; }
    }
}