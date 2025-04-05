using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class NpcResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public NpcData Data { get; set; } = null!;
}