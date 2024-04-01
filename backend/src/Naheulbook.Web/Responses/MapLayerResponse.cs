// ReSharper disable UnusedMember.Global

using System.Collections.Generic;

namespace Naheulbook.Web.Responses;

public class MapLayerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Source { get; set; } = null!;
    public bool IsGm { get; set; }
    public List<MapMarkerResponse> Markers { get; set; } = new();
}