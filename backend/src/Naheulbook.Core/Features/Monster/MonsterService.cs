using Naheulbook.Core.Features.Fight;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Monster;

public interface IMonsterService
{
    Task<MonsterEntity> GetMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CancellationToken cancellationToken = default);
    Task<MonsterEntity> CreateMonsterAsync(NaheulbookExecutionContext executionContext, int groupId, CreateMonsterRequest request, CancellationToken cancellationToken = default);
    Task<List<MonsterEntity>> GetMonstersForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default);
    Task<List<MonsterEntity>> GetDeadMonstersForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, int startIndex, int count, CancellationToken cancellationToken = default);
    Task EnsureUserCanAccessMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CancellationToken cancellationToken = default);
    Task EnsureUserCanAccessFightAsync(NaheulbookExecutionContext executionContext, int fightId, CancellationToken cancellationToken = default);
    Task<ActiveStatsModifier> AddModifierAsync(NaheulbookExecutionContext executionContext, int monsterId, ActiveStatsModifier statsModifier, CancellationToken cancellationToken = default);
    Task RemoveModifierAsync(NaheulbookExecutionContext executionContext, int monsterId, int modifierId, CancellationToken cancellationToken = default);
    Task DeleteMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CancellationToken cancellationToken = default);
    Task KillMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CancellationToken cancellationToken = default);
    Task<ItemEntity> AddItemToMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CreateItemRequest request, CancellationToken cancellationToken = default);
    Task<ItemEntity> AddRandomItemToMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CreateRandomItemRequest request, CancellationToken cancellationToken = default);
    Task UpdateMonsterDataAsync(NaheulbookExecutionContext executionContext, int monsterId, MonsterData monsterData, CancellationToken cancellationToken = default);
    Task UpdateMonsterTargetAsync(NaheulbookExecutionContext executionContext, int monsterId, TargetRequest request, CancellationToken cancellationToken = default);
    Task UpdateMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, PatchMonsterRequest request, CancellationToken cancellationToken = default);
    Task MoveMonsterToFightAsync(NaheulbookExecutionContext executionContext, int monsterId, int? fightId, CancellationToken cancellationToken = default);
}

public class MonsterService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    IActiveStatsModifierUtil activeStatsModifierUtil,
    INotificationSessionFactory notificationSessionFactory,
    IJsonUtil jsonUtil,
    ITimeService timeService,
    IItemService itemService
) : IMonsterService
{
    public async Task<MonsterEntity> GetMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var monster = await uow.Monsters.GetWithGroupWithItemsAsync(monsterId, cancellationToken);
        if (monster == null)
            throw new MonsterNotFoundException(monsterId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

        return monster;
    }

    public async Task<MonsterEntity> CreateMonsterAsync(NaheulbookExecutionContext executionContext, int groupId, CreateMonsterRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        if (request.FightId is not null)
        {
            var fight = await uow.Fights.GetAsync(request.FightId.Value, cancellationToken);
            if (fight?.GroupId != group.Id)
                throw new ForbiddenAccessException();
        }

        activeStatsModifierUtil.InitializeModifierIds(request.Modifiers);

        var monster = new MonsterEntity
        {
            Group = group,
            Name = request.Name,
            FightId = request.FightId,
            Data = jsonUtil.Serialize(request.Data),
            Modifiers = jsonUtil.Serialize(request.Modifiers),
        };

        // FIXME: test this
        uow.Monsters.Add(monster);
        monster.Items = await itemService.CreateItemsAsync(request.Items, cancellationToken);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        monster.Items = await uow.Items.GetWithAllDataByIdsAsync(monster.Items.Select(x => x.Id), cancellationToken);

        var notificationSession = notificationSessionFactory.CreateSession();

        if (request.FightId is not null)
            notificationSession.NotifyFightAddMonster(request.FightId.Value, monster);
        else
            notificationSession.NotifyGroupAddMonster(group.Id, monster);
        await notificationSession.CommitAsync();

        return monster;
    }

    public async Task<List<MonsterEntity>> GetMonstersForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        return await uow.Monsters.GetByGroupIdWithInventoryAsync(groupId, cancellationToken);
    }

    public async Task<List<MonsterEntity>> GetDeadMonstersForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, int startIndex, int count, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        return await uow.Monsters.GetDeadMonstersByGroupIdAsync(groupId, startIndex, count, cancellationToken);
    }

    public async Task EnsureUserCanAccessMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monster = await uow.Monsters.GetAsync(monsterId, cancellationToken);
        if (monster == null)
            throw new MonsterNotFoundException(monsterId);

        var group = await uow.Groups.GetGroupsWithCharactersAsync(monster.GroupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(monster.GroupId);

        // FIXME: Should be only groupOwner if monster is not in a loot
        authorizationUtil.EnsureIsGroupOwnerOrMember(executionContext, group);
    }

    public async Task EnsureUserCanAccessFightAsync(NaheulbookExecutionContext executionContext, int fightId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var fight = await uow.Fights.GetAsync(fightId, cancellationToken);
        if (fight == null)
            throw new FightNotFoundException(fightId);

        var group = await uow.Groups.GetGroupsWithCharactersAsync(fight.GroupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(fight.GroupId);

        authorizationUtil.EnsureIsGroupOwnerOrMember(executionContext, group);
    }

    public async Task<ActiveStatsModifier> AddModifierAsync(NaheulbookExecutionContext executionContext, int monsterId, ActiveStatsModifier statsModifier, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monster = await uow.Monsters.GetAsync(monsterId, cancellationToken);
        if (monster == null)
            throw new MonsterNotFoundException(monsterId);

        var group = await uow.Groups.GetAsync(monster.GroupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(monster.GroupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var modifiers = jsonUtil.Deserialize<List<ActiveStatsModifier>>(monster.Modifiers) ?? new List<ActiveStatsModifier>();
        activeStatsModifierUtil.AddModifier(modifiers, statsModifier);
        monster.Modifiers = jsonUtil.Serialize(modifiers);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyMonsterAddModifier(monster.Id, statsModifier);
        await notificationSession.CommitAsync();

        return statsModifier;
    }

    public async Task RemoveModifierAsync(NaheulbookExecutionContext executionContext, int monsterId, int modifierId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monster = await uow.Monsters.GetWithGroupAsync(monsterId, cancellationToken);
        if (monster == null)
            throw new MonsterNotFoundException(monsterId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

        var modifiers = jsonUtil.Deserialize<List<ActiveStatsModifier>>(monster.Modifiers) ?? new List<ActiveStatsModifier>();
        activeStatsModifierUtil.RemoveModifier(modifiers, modifierId);
        monster.Modifiers = jsonUtil.Serialize(modifiers);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyMonsterRemoveModifier(monster.Id, modifierId);
        await notificationSession.CommitAsync();
    }

    public async Task DeleteMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monster = await uow.Monsters.GetWithGroupAsync(monsterId, cancellationToken);
        if (monster == null)
            throw new MonsterNotFoundException(monsterId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

        var notificationSession = notificationSessionFactory.CreateSession();
        if (monster.FightId is not null)
            notificationSession.NotifyFightRemoveMonster(monster.FightId.Value, monsterId);
        else if (monster.LootId is not null)
            notificationSession.NotifyLootDeleteMonster(monster.LootId.Value, monsterId);
        else
            notificationSession.NotifyGroupDeleteMonster(monster);
        uow.Monsters.Remove(monster);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
    }

    public async Task KillMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monster = await uow.Monsters.GetWithGroupWithItemsAsync(monsterId, cancellationToken);
        if (monster == null)
            throw new MonsterNotFoundException(monsterId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

        monster.Dead = timeService.UtcNow;

        if (monster.Group.CombatLootId.HasValue)
        {
            monster.LootId = monster.Group.CombatLootId.Value;

            var notificationSession = notificationSessionFactory.CreateSession();
            notificationSession.NotifyGroupKillMonster(monster);
            notificationSession.NotifyLootAddMonster(monster.LootId.Value, monster);
            await notificationSession.CommitAsync();
        }

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
    }

    public async Task<ItemEntity> AddItemToMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CreateItemRequest request, CancellationToken cancellationToken = default)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var monster = await uow.Monsters.GetAsync(monsterId, cancellationToken);
            if (monster == null)
                throw new MonsterNotFoundException(monsterId);

            var group = await uow.Groups.GetGroupsWithCharactersAsync(monster.GroupId, cancellationToken);
            if (group == null)
                throw new GroupNotFoundException(monster.GroupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);
        }

        var item = await itemService.AddItemToAsync(ItemOwnerType.Monster, monsterId, request, cancellationToken);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyMonsterAddItem(monsterId, item);
        await session.CommitAsync();

        return item;
    }

    public async Task<ItemEntity> AddRandomItemToMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CreateRandomItemRequest request, CancellationToken cancellationToken = default)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var monster = await uow.Monsters.GetAsync(monsterId, cancellationToken);
            if (monster == null)
                throw new MonsterNotFoundException(monsterId);

            var group = await uow.Groups.GetGroupsWithCharactersAsync(monster.GroupId, cancellationToken);
            if (group == null)
                throw new GroupNotFoundException(monster.GroupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);
        }

        var item = await itemService.AddRandomItemToAsync(ItemOwnerType.Monster, monsterId, request, executionContext.UserId, cancellationToken);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyMonsterAddItem(monsterId, item);
        await session.CommitAsync();

        return item;
    }

    public async Task UpdateMonsterDataAsync(NaheulbookExecutionContext executionContext, int monsterId, MonsterData monsterData, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monster = await uow.Monsters.GetWithGroupWithItemsAsync(monsterId, cancellationToken);
        if (monster == null)
            throw new MonsterNotFoundException(monsterId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

        monster.Data = jsonUtil.Serialize(monsterData);

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyMonsterUpdateData(monster.Id, monsterData);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();
    }

    public async Task UpdateMonsterTargetAsync(NaheulbookExecutionContext executionContext, int monsterId, TargetRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monster = await uow.Monsters.GetWithGroupWithItemsAsync(monsterId, cancellationToken);
        if (monster == null)
            throw new MonsterNotFoundException(monsterId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

        if (request.IsMonster)
        {
            var targetedMonster = await uow.Monsters.GetAsync(request.Id, cancellationToken);
            if (targetedMonster == null)
                throw new TargetNotFoundException();
            if (targetedMonster.GroupId != monster.GroupId)
                throw new ForbiddenAccessException();

            monster.TargetedCharacterId = null;
            monster.TargetedMonsterId = request.Id;
        }
        else
        {
            var targetedCharacter = await uow.Characters.GetAsync(request.Id, cancellationToken);
            if (targetedCharacter == null)
                throw new TargetNotFoundException();
            if (targetedCharacter.GroupId != monster.GroupId)
                throw new ForbiddenAccessException();

            monster.TargetedMonsterId = null;
            monster.TargetedCharacterId = request.Id;
        }

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyMonsterChangeTarget(monster.Id, request);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();
    }

    public async Task UpdateMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, PatchMonsterRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monster = await uow.Monsters.GetWithGroupWithItemsAsync(monsterId, cancellationToken);
        if (monster == null)
            throw new MonsterNotFoundException(monsterId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

        monster.Name = request.Name;

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyMonsterChangeName(monster.Id, request.Name);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();
    }

    public async Task MoveMonsterToFightAsync(NaheulbookExecutionContext executionContext, int monsterId, int? fightId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monster = await uow.Monsters.GetWithGroupWithItemsAsync(monsterId, cancellationToken);
        if (monster == null)
            throw new MonsterNotFoundException(monsterId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

        if (fightId is not null)
        {
            var fight = await uow.Fights.GetAsync(fightId.Value, cancellationToken);
            if (fight?.GroupId != monster.GroupId)
                throw new ForbiddenAccessException();
        }

        var previousFightId = monster.FightId;
        monster.FightId = fightId;

        var notificationSession = notificationSessionFactory.CreateSession();
        if (fightId is not null)
        {
            if (previousFightId is not null)
                notificationSession.NotifyFightRemoveMonster(previousFightId.Value, monster.Id);
            else
                notificationSession.NotifyGroupDeleteMonster(monster);

            notificationSession.NotifyFightAddMonster(fightId.Value, monster);
        }
        else if (previousFightId is not null)
        {
            notificationSession.NotifyGroupAddMonster(monster.GroupId, monster);
            notificationSession.NotifyFightRemoveMonster(previousFightId.Value, monster.Id);
        }

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();
    }
}