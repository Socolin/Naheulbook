// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Shared.TransientModels
{
    public class ActiveStatsModifier : StatsModifier
    {
        public int Id { get; set; }
        public bool Permanent { get; set; }
        public bool Active { get; set; }

        public int? CurrentCombatCount { get; set; }
        public int? CurrentLapCount { get; set; }
        public int? CurrentTimeDuration { get; set; }

        public LapCountDecrement? LapCountDecrement { get; set; }
    }
}