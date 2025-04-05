using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MapLayerEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Source { get; set; } = null!;
    public bool IsGm { get; set; }

    public int MapId { get; set; }
    private MapEntity? _map;
    public MapEntity Map { get => _map.ThrowIfNotLoaded(); set => _map = value; }

    public int? UserId { get; set; }
    private UserEntity? _user;
    public UserEntity? User { get => _user.ThrowIfNotLoadedAndNotNull(UserId); set => _user = value; }

    public ICollection<MapMarkerEntity> Markers { get; set; } = null!;
}