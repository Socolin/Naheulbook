using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

public class OriginRequirementEntity
{
    public int Id { get; set; }

    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }

    public Guid OriginId { get; set; }
    private OriginEntity? _origin;
    public OriginEntity Origin { get => _origin.ThrowIfNotLoaded(); set => _origin = value; }

    public string StatName { get; set; } = null!;
    private StatEntity? _stat;
    public StatEntity Stat { get => _stat.ThrowIfNotLoaded(); set => _stat = value; }
}