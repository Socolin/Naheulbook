using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Extensions;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Loot;

public interface ILootService
{
    Task<LootEntity> CreateLootAsync(NaheulbookExecutionContext executionContext, int groupId, CreateLootRequest request, CancellationToken cancellationToken = default);
    Task<List<LootEntity>> GetLootsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default);
    Task EnsureUserCanAccessLootAsync(NaheulbookExecutionContext executionContext, int lootId, CancellationToken cancellationToken = default);
    Task UpdateLootVisibilityAsync(NaheulbookExecutionContext executionContext, int lootId, PutLootVisibilityRequest request, CancellationToken cancellationToken = default);
    Task DeleteLootAsync(NaheulbookExecutionContext executionContext, int lootId, CancellationToken cancellationToken = default);
    Task<ItemEntity> AddItemToLootAsync(NaheulbookExecutionContext executionContext, int lootId, CreateItemRequest request, CancellationToken cancellationToken = default);
    Task<ItemEntity> AddRandomItemToLootAsync(NaheulbookExecutionContext executionContext, int lootId, CreateRandomItemRequest request, CancellationToken cancellationToken = default);
}

public class LootService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory,
    IItemService itemService
) : ILootService
{
    public async Task<LootEntity> CreateLootAsync(NaheulbookExecutionContext executionContext, int groupId, CreateLootRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var loot = new LootEntity
        {
            Group = group,
            Name = request.Name,
            IsVisibleForPlayer = false,
            Items = new List<ItemEntity>(),
            Monsters = new List<MonsterEntity>(),
        };

        uow.Loots.Add(loot);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return loot;
    }

    public async Task<List<LootEntity>> GetLootsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        return await uow.Loots.GetByGroupIdAsync(groupId, cancellationToken);
    }

    public async Task EnsureUserCanAccessLootAsync(NaheulbookExecutionContext executionContext, int lootId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var loot = await uow.Loots.GetAsync(lootId, cancellationToken);
        if (loot == null)
            throw new LootNotFoundException(lootId);

        var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(loot.GroupId);

        authorizationUtil.EnsureIsGroupOwnerOrMember(executionContext, group);
    }

    public async Task UpdateLootVisibilityAsync(NaheulbookExecutionContext executionContext, int lootId, PutLootVisibilityRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var loot = await uow.Loots.GetAsync(lootId, cancellationToken);
        if (loot == null)
            throw new LootNotFoundException(lootId);

        var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(loot.GroupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, loot.Group);

        loot.IsVisibleForPlayer = request.VisibleForPlayer;

        var session = notificationSessionFactory.CreateSession();
        session.NotifyLootUpdateVisibility(lootId, request.VisibleForPlayer);

        if (loot.IsVisibleForPlayer)
        {
            var fullLootData = await uow.Loots.GetWithAllDataAsync(lootId, cancellationToken);
            foreach (var character in group.Characters)
                session.NotifyCharacterShowLoot(character.Id, fullLootData.NotNull());
        }
        else
        {
            foreach (var character in group.Characters)
                session.NotifyCharacterHideLoot(character.Id, loot.Id);
        }

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await session.CommitAsync();
    }

    public async Task DeleteLootAsync(NaheulbookExecutionContext executionContext, int lootId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var loot = await uow.Loots.GetAsync(lootId, cancellationToken);
        if (loot == null)
            throw new LootNotFoundException(lootId);

        var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(loot.GroupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, loot.Group);

        uow.Loots.Remove(loot);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyGroupDeleteLoot(group.Id, lootId);
        if (loot.IsVisibleForPlayer)
        {
            foreach (var character in group.Characters)
                session.NotifyCharacterHideLoot(character.Id, loot.Id);
        }

        await session.CommitAsync();
    }

    public async Task<ItemEntity> AddItemToLootAsync(NaheulbookExecutionContext executionContext, int lootId, CreateItemRequest request, CancellationToken cancellationToken = default)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var loot = await uow.Loots.GetAsync(lootId, cancellationToken);
            if (loot == null)
                throw new LootNotFoundException(lootId);

            var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId, cancellationToken);
            if (group == null)
                throw new GroupNotFoundException(loot.GroupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, loot.Group);
        }

        var item = await itemService.AddItemToAsync(ItemOwnerType.Loot, lootId, request, cancellationToken);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyLootAddItem(lootId, item);
        await session.CommitAsync();

        return item;
    }

    public async Task<ItemEntity> AddRandomItemToLootAsync(NaheulbookExecutionContext executionContext, int lootId, CreateRandomItemRequest request, CancellationToken cancellationToken = default)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var loot = await uow.Loots.GetAsync(lootId, cancellationToken);
            if (loot == null)
                throw new LootNotFoundException(lootId);

            var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId, cancellationToken);
            if (group == null)
                throw new GroupNotFoundException(loot.GroupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, loot.Group);
        }

        var item = await itemService.AddRandomItemToAsync(ItemOwnerType.Loot, lootId, request, executionContext.UserId, cancellationToken);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyLootAddItem(lootId, item);
        await session.CommitAsync();

        return item;
    }
}