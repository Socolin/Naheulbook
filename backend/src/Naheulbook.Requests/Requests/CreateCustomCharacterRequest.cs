namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateCustomCharacterRequest
{
    public required string Name { get; set; }
    public required string Sex { get; set; }
    public short FatePoint { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }

    public required BasicStatsRequest Stats { get; set; }
    public required StatsOverridesRequest BasicStatsOverrides { get; set; }

    public Guid OriginId { get; set; }
    public required IList<Guid> JobIds { get; set; }
    public required IList<Guid> SkillIds { get; set; }
    public required IDictionary<Guid, IList<Guid>> SpecialityIds { get; set; }

    public bool IsNpc { get; set; }
    public int? GroupId { get; set; }

    [PublicAPI]
    public class BasicStatsRequest
    {
        public int Ad { get; set; }
        public int Cou { get; set; }
        public int Cha { get; set; }
        public int Fo { get; set; }
        public int Int { get; set; }
    }

    [PublicAPI]
    public class StatsOverridesRequest
    {
        public int? Ad { get; set; }
        public int? Prd { get; set; }
        public int? Ev { get; set; }
        public int? Ea { get; set; }
    }
}