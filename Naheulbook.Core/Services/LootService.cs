using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface ILootService
    {
        Task<Loot> CreateLootAsync(NaheulbookExecutionContext executionContext, int groupId, CreateLootRequest request);
        Task<List<Loot>> GetLootsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId);
        Task EnsureUserCanAccessLootAsync(NaheulbookExecutionContext executionContext, int lootId);
        Task UpdateLootVisibilityAsync(NaheulbookExecutionContext executionContext, int lootId, PutLootVisibilityRequest request);
    }

    public class LootService : ILootService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly INotificationSessionFactory _notificationSessionFactory;

        public LootService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil,
            INotificationSessionFactory notificationSessionFactory
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
            _notificationSessionFactory = notificationSessionFactory;
        }

        public async Task<Loot> CreateLootAsync(NaheulbookExecutionContext executionContext, int groupId, CreateLootRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                var loot = new Loot
                {
                    Group = group,
                    Name = request.Name,
                    IsVisibleForPlayer = false
                };

                uow.Loots.Add(loot);
                await uow.CompleteAsync();

                return loot;
            }
        }

        public async Task<List<Loot>> GetLootsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                return await uow.Loots.GetByGroupIdAsync(groupId);
            }
        }

        public async Task EnsureUserCanAccessLootAsync(NaheulbookExecutionContext executionContext, int lootId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var loot = await uow.Loots.GetAsync(lootId);
                if (loot == null)
                    throw new LootNotFoundException(lootId);

                var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId);
                if (group == null)
                    throw new GroupNotFoundException(loot.GroupId);

                _authorizationUtil.EnsureIsGroupOwnerOrMember(executionContext, group);
            }
        }

        public async Task UpdateLootVisibilityAsync(NaheulbookExecutionContext executionContext, int lootId, PutLootVisibilityRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var loot = await uow.Loots.GetAsync(lootId);
                if (loot == null)
                    throw new LootNotFoundException(lootId);

                var group = await uow.Groups.GetGroupsWithCharactersAsync(loot.GroupId);
                if (group == null)
                    throw new GroupNotFoundException(loot.GroupId);

                _authorizationUtil.EnsureIsGroupOwnerOrMember(executionContext, loot.Group);

                loot.IsVisibleForPlayer = request.VisibleForPlayer;

                var session = _notificationSessionFactory.CreateSession();
                session.NotifyLootUpdateVisibility(lootId, request.VisibleForPlayer);

                if (loot.IsVisibleForPlayer)
                {
                    var fullLootData = await uow.Loots.GetWithAllDataAsync(lootId);
                    foreach (var character in group.Characters)
                        session.NotifyCharacterShowLoot(character.Id, fullLootData);
                }
                else
                {
                    foreach (var character in group.Characters)
                        session.NotifyCharacterHideLoot(character.Id, loot.Id);
                }

                await uow.CompleteAsync();
                await session.CommitAsync();
            }
        }
    }
}