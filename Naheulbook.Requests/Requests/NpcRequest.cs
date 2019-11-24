using Naheulbook.Shared.TransientModels;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Requests.Requests
{
    public class NpcRequest
    {
        public string Name { get; set; } = null!;
        public NpcData Data { get; set; } = null!;
    }
}