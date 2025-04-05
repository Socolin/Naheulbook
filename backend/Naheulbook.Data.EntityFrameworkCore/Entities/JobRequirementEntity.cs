using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class JobRequirementEntity
{
    public int Id { get; set; }

    public long? MinValue { get; set; }
    public long? MaxValue { get; set; }

    public string StatName { get; set; } = null!;
    private StatEntity? _stat;
    public StatEntity Stat { get => _stat.ThrowIfNotLoaded(); set => _stat = value; }

    public Guid JobId { get; set; }
    private JobEntity? _job;
    public JobEntity Job { get => _job.ThrowIfNotLoaded(); set => _job = value; }
}