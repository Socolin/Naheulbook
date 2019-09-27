using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Stat
    {
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Bonus { get; set; }
        public string? Penalty { get; set; }

        public IEnumerable<JobRequirement> JobRequirements { get; set; } = null!;
        public IEnumerable<EffectModifier> Effects { get; set; } = null!;
    }
}