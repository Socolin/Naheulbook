using System.Collections.Generic;

namespace Naheulbook.Requests.Requests
{
    public class CreateEffectRequest
    {
        public int CombatCount { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public short? Dice { get; set; }
        public int? LapCount { get; set; }
        public string Name { get; set; }
        public string DurationType { get; set; }
        public int TimeDuration { get; set; }
        public List<CreateEffectModifierRequest> Modifiers { get; set; }
    }

    public class CreateEffectModifierRequest
    {
        public string Stat { get; set; }
        public short Value { get; set; }
        public string Type { get; set; }
    }
}