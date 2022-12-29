using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Web.Responses;

public class MapResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public MapData Data { get; set; } = null!;
    public MapImageData ImageData { get; set; } = null!;
    public IList<MapLayerResponse> Layers { get; set; } = null!;
}