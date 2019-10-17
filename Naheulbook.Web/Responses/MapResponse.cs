using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Web.Responses
{
    public class MapResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public MapData Data { get; set; } = null!;
    }
}