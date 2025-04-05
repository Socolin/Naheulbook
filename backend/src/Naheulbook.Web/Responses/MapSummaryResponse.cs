using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class MapSummaryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public MapData Data { get; set; } = null!;
}