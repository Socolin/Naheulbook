using System;
using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Requests.Requests
{
    public class CreateCustomCharacterRequest
    {
        public string Name { get; set; } = null!;
        public string Sex { get; set; } = null!;
        public short FatePoint { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }

        public BasicStats Stats { get; set; } = null!;
        public StatsOverrides BasicStatsOverrides { get; set; } = null!;

        public Guid OriginId { get; set; }
        public IList<int> JobIds { get; set; } = null!;
        public IList<Guid> SkillIds { get; set; } = null!;
        public IDictionary<int, IList<int>> SpecialityIds { get; set; } = null!;

        public bool IsNpc { get; set; }
        public int? GroupId { get; set; }

        public class BasicStats
        {
            public int Ad { get; set; }
            public int Cou { get; set; }
            public int Cha { get; set; }
            public int Fo { get; set; }
            public int Int { get; set; }
        }

        public class StatsOverrides
        {
            public int? Ad { get; set; }
            public int? Prd { get; set; }
            public int? Ev { get; set; }
            public int? Ea { get; set; }
        }
    }
}