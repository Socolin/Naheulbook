using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class AptitudeEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Roll { get; set; }
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Effect { get; set; } = null!;

    public Guid AptitudeGroupId { get; set; }
    private AptitudeGroupEntity? _aptitudeGroup;
    public AptitudeGroupEntity AptitudeGroup { get => _aptitudeGroup.ThrowIfNotLoaded(); set => _aptitudeGroup = value; }
}