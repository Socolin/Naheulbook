using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class AptitudeGroupEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    private ICollection<AptitudeEntity>? _aptitudes;
    public ICollection<AptitudeEntity> Aptitudes { get => _aptitudes.ThrowIfNotLoaded(); set => _aptitudes = value; }
}