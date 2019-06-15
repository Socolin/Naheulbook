using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests
{
    public class CreateEffectRequest
    {
        public int? CombatCount { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public short? Dice { get; set; }
        public int? LapCount { get; set; }
        public string Name { get; set; }
        public string DurationType { get; set; }
        public int? TimeDuration { get; set; }
        public List<StatModifier> Modifiers { get; set; }
    }
}