using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class OriginInfoEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    public Guid OriginId { get; set; }
    private OriginEntity? _origin;
    public OriginEntity Origin { get => _origin.ThrowIfNotLoaded(); set => _origin = value; }
}