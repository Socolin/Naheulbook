using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Services
{
    public interface IMonsterService
    {
        Task<MonsterEntity> GetMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId);
        Task<MonsterEntity> CreateMonsterAsync(NaheulbookExecutionContext executionContext, int groupId, CreateMonsterRequest request);
        Task<List<MonsterEntity>> GetMonstersForGroupAsync(NaheulbookExecutionContext executionContext, int groupId);
        Task<List<MonsterEntity>> GetDeadMonstersForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, int startIndex, int count);
        Task EnsureUserCanAccessMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId);
        Task<ActiveStatsModifier> AddModifierAsync(NaheulbookExecutionContext executionContext, int monsterId, ActiveStatsModifier statsModifier);
        Task RemoveModifierAsync(NaheulbookExecutionContext executionContext, int monsterId, int modifierId);
        Task DeleteMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId);
        Task KillMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId);
        Task<ItemEntity> AddItemToMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CreateItemRequest request);
        Task<ItemEntity> AddRandomItemToMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CreateRandomItemRequest request);
        Task UpdateMonsterDataAsync(NaheulbookExecutionContext executionContext, int monsterId, MonsterData monsterData);
        Task UpdateMonsterTargetAsync(NaheulbookExecutionContext executionContext, int monsterId, TargetRequest request);
        Task UpdateMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, PatchMonsterRequest request);
    }

    public class MonsterService : IMonsterService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IActiveStatsModifierUtil _activeStatsModifierUtil;
        private readonly INotificationSessionFactory _notificationSessionFactory;
        private readonly IJsonUtil _jsonUtil;
        private readonly ITimeService _timeService;
        private readonly IItemService _itemService;

        public MonsterService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil,
            IActiveStatsModifierUtil activeStatsModifierUtil,
            INotificationSessionFactory notificationSessionFactory,
            IJsonUtil jsonUtil,
            ITimeService timeService,
            IItemService itemService
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
            _activeStatsModifierUtil = activeStatsModifierUtil;
            _notificationSessionFactory = notificationSessionFactory;
            _jsonUtil = jsonUtil;
            _timeService = timeService;
            _itemService = itemService;
        }

        public async Task<MonsterEntity> GetMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monster = await uow.Monsters.GetWithGroupAsync(monsterId);
                if (monster == null)
                    throw new MonsterNotFoundException(monsterId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

                return monster;
            }
        }

        public async Task<MonsterEntity> CreateMonsterAsync(NaheulbookExecutionContext executionContext, int groupId, CreateMonsterRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                _activeStatsModifierUtil.InitializeModifierIds(request.Modifiers);

                var monster = new MonsterEntity
                {
                    Group = group,
                    Name = request.Name,
                    Data = _jsonUtil.Serialize(request.Data),
                    Modifiers = _jsonUtil.Serialize(request.Modifiers)
                };

                // FIXME: test this
                uow.Monsters.Add(monster);
                monster.Items = await _itemService.CreateItemsAsync(request.Items);
                await uow.SaveChangesAsync();

                monster.Items = await uow.Items.GetWithAllDataByIdsAsync(monster.Items.Select(x => x.Id));

                var notificationSession = _notificationSessionFactory.CreateSession();
                notificationSession.NotifyGroupAddMonster(group.Id, monster);
                await notificationSession.CommitAsync();

                return monster;
            }
        }

        public async Task<List<MonsterEntity>> GetMonstersForGroupAsync(NaheulbookExecutionContext executionContext, int groupId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                return await uow.Monsters.GetByGroupIdWithInventoryAsync(groupId);
            }
        }

        public async Task<List<MonsterEntity>> GetDeadMonstersForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, int startIndex, int count)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                return await uow.Monsters.GetDeadMonstersByGroupIdAsync(groupId, startIndex, count);
            }
        }

        public async Task EnsureUserCanAccessMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monster = await uow.Monsters.GetAsync(monsterId);
                if (monster == null)
                    throw new MonsterNotFoundException(monsterId);

                var group = await uow.Groups.GetGroupsWithCharactersAsync(monster.GroupId);
                if (group == null)
                    throw new GroupNotFoundException(monster.GroupId);

                // FIXME: Should be only groupOwner if monster is not in a loot
                _authorizationUtil.EnsureIsGroupOwnerOrMember(executionContext, group);
            }
        }

        public async Task<ActiveStatsModifier> AddModifierAsync(NaheulbookExecutionContext executionContext, int monsterId, ActiveStatsModifier statsModifier)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monster = await uow.Monsters.GetAsync(monsterId);
                if (monster == null)
                    throw new MonsterNotFoundException(monsterId);

                var group = await uow.Groups.GetAsync(monster.GroupId);
                if (group == null)
                    throw new GroupNotFoundException(monster.GroupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                var modifiers = _jsonUtil.Deserialize<List<ActiveStatsModifier>>(monster.Modifiers) ?? new List<ActiveStatsModifier>();
                _activeStatsModifierUtil.AddModifier(modifiers, statsModifier);
                monster.Modifiers = _jsonUtil.Serialize(modifiers);

                await uow.SaveChangesAsync();

                var notificationSession = _notificationSessionFactory.CreateSession();
                notificationSession.NotifyMonsterAddModifier(monster.Id, statsModifier);
                await notificationSession.CommitAsync();

                return statsModifier;
            }
        }

        public async Task RemoveModifierAsync(NaheulbookExecutionContext executionContext, int monsterId, int modifierId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monster = await uow.Monsters.GetWithGroupAsync(monsterId);
                if (monster == null)
                    throw new MonsterNotFoundException(monsterId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

                var modifiers = _jsonUtil.Deserialize<List<ActiveStatsModifier>>(monster.Modifiers) ?? new List<ActiveStatsModifier>();
                _activeStatsModifierUtil.RemoveModifier(modifiers, modifierId);
                monster.Modifiers = _jsonUtil.Serialize(modifiers);

                await uow.SaveChangesAsync();

                var notificationSession = _notificationSessionFactory.CreateSession();
                notificationSession.NotifyMonsterRemoveModifier(monster.Id, modifierId);
                await notificationSession.CommitAsync();
            }
        }

        public async Task DeleteMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monster = await uow.Monsters.GetWithGroupAsync(monsterId);
                if (monster == null)
                    throw new MonsterNotFoundException(monsterId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

                uow.Monsters.Remove(monster);

                await uow.SaveChangesAsync();
            }
        }

        public async Task KillMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monster = await uow.Monsters.GetWithGroupWithItemsAsync(monsterId);
                if (monster == null)
                    throw new MonsterNotFoundException(monsterId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

                monster.Dead = _timeService.UtcNow;

                if (monster.Group.CombatLootId.HasValue)
                {
                    monster.LootId = monster.Group.CombatLootId.Value;

                    var notificationSession = _notificationSessionFactory.CreateSession();
                    notificationSession.NotifyGroupKillMonster(monster);
                    notificationSession.NotifyLootAddMonster(monster.LootId.Value, monster);
                    await notificationSession.CommitAsync();
                }

                await uow.SaveChangesAsync();
            }
        }

        public async Task<ItemEntity> AddItemToMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CreateItemRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monster = await uow.Monsters.GetAsync(monsterId);
                if (monster == null)
                    throw new MonsterNotFoundException(monsterId);

                var group = await uow.Groups.GetGroupsWithCharactersAsync(monster.GroupId);
                if (group == null)
                    throw new GroupNotFoundException(monster.GroupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);
            }

            var item = await _itemService.AddItemToAsync(ItemOwnerType.Monster, monsterId, request);

            var session = _notificationSessionFactory.CreateSession();
            session.NotifyMonsterAddItem(monsterId, item);
            await session.CommitAsync();

            return item;
        }

        public async Task<ItemEntity> AddRandomItemToMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, CreateRandomItemRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monster = await uow.Monsters.GetAsync(monsterId);
                if (monster == null)
                    throw new MonsterNotFoundException(monsterId);

                var group = await uow.Groups.GetGroupsWithCharactersAsync(monster.GroupId);
                if (group == null)
                    throw new GroupNotFoundException(monster.GroupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);
            }

            var item = await _itemService.AddRandomItemToAsync(ItemOwnerType.Monster, monsterId, request, executionContext.UserId);

            var session = _notificationSessionFactory.CreateSession();
            session.NotifyMonsterAddItem(monsterId, item);
            await session.CommitAsync();

            return item;
        }

        public async Task UpdateMonsterDataAsync(NaheulbookExecutionContext executionContext, int monsterId, MonsterData monsterData)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monster = await uow.Monsters.GetWithGroupWithItemsAsync(monsterId);
                if (monster == null)
                    throw new MonsterNotFoundException(monsterId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

                monster.Data = _jsonUtil.Serialize(monsterData);

                var notificationSession = _notificationSessionFactory.CreateSession();
                notificationSession.NotifyMonsterUpdateData(monster.Id, monsterData);

                await uow.SaveChangesAsync();
                await notificationSession.CommitAsync();
            }
        }

        public async Task UpdateMonsterTargetAsync(NaheulbookExecutionContext executionContext, int monsterId, TargetRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monster = await uow.Monsters.GetWithGroupWithItemsAsync(monsterId);
                if (monster == null)
                    throw new MonsterNotFoundException(monsterId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

                if (request.IsMonster)
                {
                    var targetedMonster = await uow.Monsters.GetAsync(request.Id);
                    if (targetedMonster == null)
                        throw new TargetNotFoundException();
                    if (targetedMonster.GroupId != monster.GroupId)
                        throw new ForbiddenAccessException();

                    monster.TargetedCharacterId = null;
                    monster.TargetedMonsterId = request.Id;
                }
                else
                {
                    var targetedCharacter = await uow.Characters.GetAsync(request.Id);
                    if (targetedCharacter == null)
                        throw new TargetNotFoundException();
                    if (targetedCharacter.GroupId != monster.GroupId)
                        throw new ForbiddenAccessException();

                    monster.TargetedMonsterId = null;
                    monster.TargetedCharacterId = request.Id;
                }

                var notificationSession = _notificationSessionFactory.CreateSession();
                notificationSession.NotifyMonsterChangeTarget(monster.Id, request);

                await uow.SaveChangesAsync();
                await notificationSession.CommitAsync();
            }
        }

        public async Task UpdateMonsterAsync(NaheulbookExecutionContext executionContext, int monsterId, PatchMonsterRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monster = await uow.Monsters.GetWithGroupWithItemsAsync(monsterId);
                if (monster == null)
                    throw new MonsterNotFoundException(monsterId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, monster.Group);

                monster.Name = request.Name;

                var notificationSession = _notificationSessionFactory.CreateSession();
                notificationSession.NotifyMonsterChangeName(monster.Id, request.Name);

                await uow.SaveChangesAsync();
                await notificationSession.CommitAsync();
            }
        }
    }
}