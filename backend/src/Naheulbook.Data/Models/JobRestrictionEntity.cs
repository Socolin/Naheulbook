using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

public class JobRestrictionEntity
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public string? Flags { get; set; }

    public Guid JobId { get; set; }
    private JobEntity? _job;
    public JobEntity Job { get => _job.ThrowIfNotLoaded(); set => _job = value; }
}