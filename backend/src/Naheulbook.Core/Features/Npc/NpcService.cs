using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Npc;

public interface INpcService
{
    Task<List<NpcEntity>> LoadNpcsAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default);
    Task<NpcEntity> CreateNpcAsync(NaheulbookExecutionContext executionContext, int groupId, NpcRequest request, CancellationToken cancellationToken = default);
    Task<NpcEntity> EditNpcAsync(NaheulbookExecutionContext executionContext, int npcId, NpcRequest request, CancellationToken cancellationToken = default);
}

public class NpcService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    IJsonUtil jsonUtil
) : INpcService
{
    public async Task<List<NpcEntity>> LoadNpcsAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        return await uow.Npcs.GetByGroupIdAsync(groupId, cancellationToken);
    }

    public async Task<NpcEntity> CreateNpcAsync(NaheulbookExecutionContext executionContext, int groupId, NpcRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
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

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return npc;
    }

    public async Task<NpcEntity> EditNpcAsync(NaheulbookExecutionContext executionContext, int npcId, NpcRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var npc = await uow.Npcs.GetWitGroupAsync(npcId, cancellationToken);
        if (npc == null)
            throw new NpcNotFoundException(npcId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, npc.Group);

        npc.Name = request.Name;
        npc.Data = jsonUtil.SerializeNonNull(request.Data);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return npc;
    }
}