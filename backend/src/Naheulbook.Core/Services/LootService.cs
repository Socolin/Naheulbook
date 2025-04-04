using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Services;

public interface ILootService
{
    Task<LootEntity> CreateLootAsync(NaheulbookExecutionContext executionContext, int groupId, CreateLootRequest request);
    Task<List<LootEntity>> GetLootsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId);
    Task EnsureUserCanAccessLootAsync(NaheulbookExecutionContext executionContext, int lootId);
    Task UpdateLootVisibilityAsync(NaheulbookExecutionContext executionContext, int lootId, PutLootVisibilityRequest request);
    Task DeleteLootAsync(NaheulbookExecutionContext executionContext, int lootId);
    Task<ItemEntity> AddItemToLootAsync(NaheulbookExecutionContext executionContext, int lootId, CreateItemRequest request);
    Task<ItemEntity> AddRandomItemToLootAsync(NaheulbookExecutionContext executionContext, int lootId, CreateRandomItemRequest request);
}

public class LootService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory,
    IItemService itemService
) : ILootService
{
    public async Task<LootEntity> CreateLootAsync(NaheulbookExecutionContext executionContext, int groupId, CreateLootRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId);
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
        await uow.SaveChangesAsync();

        return loot;
    }

    public async Task<List<LootEntity>> GetLootsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var group = await uow.Groups.GetAsync(groupId);
            if (group == null)
                throw new GroupNotFoundException(groupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, group);

            return await uow.Loots.GetByGroupIdAsync(groupId);
        }
    }

    public async Task EnsureUserCanAccessLootAsync(NaheulbookExecutionContext executionContext, int lootId)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var loot = await uow.Loots.GetAsync(lootId);
            if (loot == null)
                throw new LootNotFoundException(lootId);

            var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId);
            if (group == null)
                throw new GroupNotFoundException(loot.GroupId);

            authorizationUtil.EnsureIsGroupOwnerOrMember(executionContext, group);
        }
    }

    public async Task UpdateLootVisibilityAsync(NaheulbookExecutionContext executionContext, int lootId, PutLootVisibilityRequest request)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var loot = await uow.Loots.GetAsync(lootId);
            if (loot == null)
                throw new LootNotFoundException(lootId);

            var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId);
            if (group == null)
                throw new GroupNotFoundException(loot.GroupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, loot.Group);

            loot.IsVisibleForPlayer = request.VisibleForPlayer;

            var session = notificationSessionFactory.CreateSession();
            session.NotifyLootUpdateVisibility(lootId, request.VisibleForPlayer);

            if (loot.IsVisibleForPlayer)
            {
                var fullLootData = await uow.Loots.GetWithAllDataAsync(lootId);
                foreach (var character in group.Characters)
                    session.NotifyCharacterShowLoot(character.Id, fullLootData.NotNull());
            }
            else
            {
                foreach (var character in group.Characters)
                    session.NotifyCharacterHideLoot(character.Id, loot.Id);
            }

            await uow.SaveChangesAsync();
            await session.CommitAsync();
        }
    }

    public async Task DeleteLootAsync(NaheulbookExecutionContext executionContext, int lootId)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var loot = await uow.Loots.GetAsync(lootId);
            if (loot == null)
                throw new LootNotFoundException(lootId);

            var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId);
            if (group == null)
                throw new GroupNotFoundException(loot.GroupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, loot.Group);

            uow.Loots.Remove(loot);
            await uow.SaveChangesAsync();

            var session = notificationSessionFactory.CreateSession();
            session.NotifyGroupDeleteLoot(group.Id, lootId);
            if (loot.IsVisibleForPlayer)
            {
                foreach (var character in group.Characters)
                    session.NotifyCharacterHideLoot(character.Id, loot.Id);
            }

            await session.CommitAsync();
        }
    }

    public async Task<ItemEntity> AddItemToLootAsync(NaheulbookExecutionContext executionContext, int lootId, CreateItemRequest request)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var loot = await uow.Loots.GetAsync(lootId);
            if (loot == null)
                throw new LootNotFoundException(lootId);

            var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId);
            if (group == null)
                throw new GroupNotFoundException(loot.GroupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, loot.Group);
        }

        var item = await itemService.AddItemToAsync(ItemOwnerType.Loot, lootId, request);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyLootAddItem(lootId, item);
        await session.CommitAsync();

        return item;
    }

    public async Task<ItemEntity> AddRandomItemToLootAsync(NaheulbookExecutionContext executionContext, int lootId, CreateRandomItemRequest request)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var loot = await uow.Loots.GetAsync(lootId);
            if (loot == null)
                throw new LootNotFoundException(lootId);

            var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId);
            if (group == null)
                throw new GroupNotFoundException(loot.GroupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, loot.Group);
        }

        var item = await itemService.AddRandomItemToAsync(ItemOwnerType.Loot, lootId, request, executionContext.UserId);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyLootAddItem(lootId, item);
        await session.CommitAsync();

        return item;
    }
}