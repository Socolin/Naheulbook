using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Features.Fight;

public interface IFightService
{
    Task<List<FightEntity>> GetFightsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId);
    Task<FightEntity> CreateFightAsync(NaheulbookExecutionContext executionContext, int groupId, CreateFightRequest request);
    Task DeleteFightAsync(NaheulbookExecutionContext executionContext, int groupId, int fightId);
    Task StartFightAsync(NaheulbookExecutionContext executionContext, int groupId, int fightId);
}

public class FightService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory
) : IFightService
{
    public async Task<List<FightEntity>> GetFightsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        return await uow.Fights.GetByGroupIdWithMonstersAsync(groupId);
    }

    public async Task<FightEntity> CreateFightAsync(NaheulbookExecutionContext executionContext, int groupId, CreateFightRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId);
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
        await uow.SaveChangesAsync();

        var session = notificationSessionFactory.CreateSession();
        session.NotifyGroupAddFight(groupId, fight);
        await session.CommitAsync();

        return fight;
    }

    public async Task DeleteFightAsync(NaheulbookExecutionContext executionContext, int groupId, int fightId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var fight = await uow.Fights.GetAsync(fightId);
        if (fight == null || fight.GroupId != group.Id)
            throw new FightNotFoundException(fightId);

        uow.Fights.Remove(fight);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyGroupDeleteFight(groupId, fightId);
        await uow.SaveChangesAsync();
        await session.CommitAsync();
    }

    public async Task StartFightAsync(NaheulbookExecutionContext executionContext, int groupId, int fightId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var fight = await uow.Fights.GetWithMonstersAsync(fightId);
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
        await uow.SaveChangesAsync();
        await session.CommitAsync();
    }
}