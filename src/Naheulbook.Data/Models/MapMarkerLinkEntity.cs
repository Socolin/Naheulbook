using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

public class MapMarkerLinkEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public int MapMarkerId { get; set; }
    private MapMarkerEntity? _mapMarker;
    public MapMarkerEntity MapMarker { get => _mapMarker.ThrowIfNotLoaded(); set => _mapMarker = value; }

    public int TargetMapId { get; set; }
    private MapEntity? _targetMap;
    public MapEntity TargetMap { get => _targetMap.ThrowIfNotLoaded(); set => _targetMap = value; }

    public int? TargetMapMarkerId { get; set; }
    private MapMarkerEntity? _targetMapMarker;
    public MapMarkerEntity? TargetMapMarker { get => _targetMapMarker.ThrowIfNotLoadedAndNotNull(TargetMapMarkerId); set => _targetMapMarker = value; }
}