using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Fight;

public interface IFightService
{
    Task<List<FightEntity>> GetFightsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default);
    Task<FightEntity> CreateFightAsync(NaheulbookExecutionContext executionContext, int groupId, CreateFightRequest request, CancellationToken cancellationToken = default);
    Task DeleteFightAsync(NaheulbookExecutionContext executionContext, int groupId, int fightId, CancellationToken cancellationToken = default);
    Task StartFightAsync(NaheulbookExecutionContext executionContext, int groupId, int fightId, CancellationToken cancellationToken = default);
}

public class FightService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory
) : IFightService
{
    public async Task<List<FightEntity>> GetFightsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        return await uow.Fights.GetByGroupIdWithMonstersAsync(groupId, cancellationToken);
    }

    public async Task<FightEntity> CreateFightAsync(NaheulbookExecutionContext executionContext, int groupId, CreateFightRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var fight = new FightEntity
        {
            Name = request.Name,
            GroupId = groupId,
            Monsters = [],
        };

        uow.Fights.Add(fight);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyGroupAddFight(groupId, fight);
        await session.CommitAsync();

        return fight;
    }

    public async Task DeleteFightAsync(NaheulbookExecutionContext executionContext, int groupId, int fightId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var fight = await uow.Fights.GetAsync(fightId, cancellationToken);
        if (fight == null || fight.GroupId != group.Id)
            throw new FightNotFoundException(fightId);

        uow.Fights.Remove(fight);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyGroupDeleteFight(groupId, fightId);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await session.CommitAsync();
    }

    public async Task StartFightAsync(NaheulbookExecutionContext executionContext, int groupId, int fightId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var fight = await uow.Fights.GetWithMonstersAsync(fightId, cancellationToken);
        if (fight == null || fight.GroupId != group.Id)
            throw new FightNotFoundException(fightId);

        var session = notificationSessionFactory.CreateSession();
        foreach (var monster in fight.Monsters)
        {
            monster.FightId = null;
            monster.Fight = null;
            session.NotifyGroupAddMonster(fight.GroupId, monster);
        }

        uow.Fights.Remove(fight);
        session.NotifyGroupDeleteFight(groupId, fightId);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await session.CommitAsync();
    }
}