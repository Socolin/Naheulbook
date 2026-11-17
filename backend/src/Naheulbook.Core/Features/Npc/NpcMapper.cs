using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Npc;

public interface INpcMapper
{
    NpcResponse ToResponse(NpcEntity npc);
}

public class NpcMapper : INpcMapper
{
    public NpcResponse ToResponse(NpcEntity npc)
    {
        return new NpcResponse
        {
            Id = npc.Id,
            Name = npc.Name,
            Data = MapperHelpers.FromJson<NpcData>(npc.Data)!,
        };
    }
}
