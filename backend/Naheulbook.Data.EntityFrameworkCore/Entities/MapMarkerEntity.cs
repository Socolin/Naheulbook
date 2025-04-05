using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MapMarkerEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Type { get; set; } = null!;
    public string MarkerInfo { get; set; } = null!;

    public int LayerId { get; set; }
    private MapLayerEntity? _layer;
    public MapLayerEntity Layer { get => _layer.ThrowIfNotLoaded(); set => _layer = value; }

    private ICollection<MapMarkerLinkEntity>? _links;
    public ICollection<MapMarkerLinkEntity> Links { get => _links.ThrowIfNotLoaded(); set => _links = value; }
}