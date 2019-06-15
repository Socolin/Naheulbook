using System.Collections.Generic;

namespace Naheulbook.Shared.TransientModels
{
    public class StatsModifier
    {
        public string Name { get; set; }

        public bool Reusable { get; set; }

        public string DurationType { get; set; }
        public string Duration { get; set; }
        public int? CombatCount { get; set; }
        public int? LapCount { get; set; }
        public int? TimeDuration { get; set; }

        public string Description { get; set; }
        public string Type { get; set; }

        public List<StatModifier> Values { get; set; }
    }
}