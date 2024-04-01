using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Services;

public interface INpcService
{
    Task<List<NpcEntity>> LoadNpcsAsync(NaheulbookExecutionContext executionContext, int groupId);
    Task<NpcEntity> CreateNpcAsync(NaheulbookExecutionContext executionContext, int groupId, NpcRequest request);
    Task<NpcEntity> EditNpcAsync(NaheulbookExecutionContext executionContext, int npcId, NpcRequest request);
}

public class NpcService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    IJsonUtil jsonUtil
) : INpcService
{
    public async Task<List<NpcEntity>> LoadNpcsAsync(NaheulbookExecutionContext executionContext, int groupId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        return await uow.Npcs.GetByGroupIdAsync(groupId);
    }

    public async Task<NpcEntity> CreateNpcAsync(NaheulbookExecutionContext executionContext, int groupId, NpcRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var npc = new NpcEntity
        {
            GroupId = groupId,
            Name = request.Name,
            Data = jsonUtil.SerializeNonNull(request.Data),
        };

        uow.Npcs.Add(npc);

        await uow.SaveChangesAsync();

        return npc;
    }

    public async Task<NpcEntity> EditNpcAsync(NaheulbookExecutionContext executionContext, int npcId, NpcRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var npc = await uow.Npcs.GetWitGroupAsync(npcId);
        if (npc == null)
            throw new NpcNotFoundException(npcId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, npc.Group);

        npc.Name = request.Name;
        npc.Data = jsonUtil.SerializeNonNull(request.Data);

        await uow.SaveChangesAsync();

        return npc;
    }
}