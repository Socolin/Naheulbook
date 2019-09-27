using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests
{
    public class AddCharacterModifierRequest
    {
        public string Name { get; set; } = null!;

        public bool Reusable { get; set; }

        public string DurationType { get; set; } = null!;
        public string? Duration { get; set; }
        public int? CombatCount { get; set; }
        public int? LapCount { get; set; }
        public int? TimeDuration { get; set; }

        public string? Description { get; set; }
        public string? Type { get; set; }

        public List<StatModifier> Values { get; set; } = new List<StatModifier>();

        public LapCountDecrement? LapCountDecrement { get; set; }
    }
}