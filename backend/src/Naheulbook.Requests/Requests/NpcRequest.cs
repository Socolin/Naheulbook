using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class NpcRequest
{
    public required string Name { get; set; }
    public required NpcData Data { get; set; }
}