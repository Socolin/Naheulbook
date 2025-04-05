using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class MapResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public MapData Data { get; set; } = null!;
    public MapImageData ImageData { get; set; } = null!;
    public IList<MapLayerResponse> Layers { get; set; } = null!;
}