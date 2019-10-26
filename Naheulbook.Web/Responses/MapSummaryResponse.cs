using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Web.Responses
{
    public class MapSummaryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public MapData Data { get; set; } = null!;
    }
}