using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Services
{
    public interface IGroupService
    {
        Task<Group> CreateGroupAsync(NaheulbookExecutionContext executionContext, CreateGroupRequest request);
        Task<List<Group>> GetGroupListAsync(NaheulbookExecutionContext executionContext);
        Task<Group> GetGroupDetailsAsync(NaheulbookExecutionContext executionContext, int groupId);
        Task EditGroupPropertiesAsync(NaheulbookExecutionContext executionContext, int groupId, PatchGroupRequest request);
        Task<List<GroupHistoryEntry>> GetGroupHistoryEntriesAsync(NaheulbookExecutionContext executionContext, int groupId, int page);
        Task EnsureUserCanAccessGroupAsync(NaheulbookExecutionContext executionContext, int groupId);
        Task<GroupInvite> CreateInviteAsync(NaheulbookExecutionContext executionContext, int groupId, CreateInviteRequest request);
        Task<GroupInvite> CancelOrRejectInviteAsync(NaheulbookExecutionContext executionContext, int groupId, int characterId);
        Task AcceptInviteAsync(NaheulbookExecutionContext executionContext, int groupId, int characterId);
        Task StartCombatAsync(NaheulbookExecutionContext executionContext, int groupId);
        Task EndCombatAsync(NaheulbookExecutionContext executionContext, int groupId);
        Task UpdateDurationsAsync(NaheulbookExecutionContext executionContext, int groupId, IList<PostGroupUpdateDurationsRequest> request);
        Task<IEnumerable<Character>> ListActiveCharactersAsync(NaheulbookExecutionContext executionContext, int groupId);
        Task<NhbkDate> AddTimeAsync(NaheulbookExecutionContext executionContext, int groupId, NhbkDateOffset request);
        Task AddHistoryEntryAsync(NaheulbookExecutionContext executionContext, int groupId, PostCreateGroupHistoryEntryRequest request);
        Task EditGroupConfigAsync(NaheulbookExecutionContext executionContext, int groupId, PatchGroupConfigRequest request);
    }

    public class GroupService : IGroupService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IMapper _mapper;
        private readonly INotificationSessionFactory _notificationSessionFactory;
        private readonly IDurationUtil _durationUtil;
        private readonly IGroupUtil _groupUtil;
        private readonly IGroupHistoryUtil _groupHistoryUtil;
        private readonly IGroupConfigUtil _groupConfigUtil;

        public GroupService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil,
            INotificationSessionFactory notificationSessionFactory,
            IMapper mapper,
            IDurationUtil durationUtil,
            IGroupUtil groupUtil,
            IGroupHistoryUtil groupHistoryUtil,
            IGroupConfigUtil groupConfigUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
            _mapper = mapper;
            _notificationSessionFactory = notificationSessionFactory;
            _durationUtil = durationUtil;
            _groupUtil = groupUtil;
            _groupHistoryUtil = groupHistoryUtil;
            _groupConfigUtil = groupConfigUtil;
        }

        public async Task<Group> CreateGroupAsync(NaheulbookExecutionContext executionContext, CreateGroupRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = new Group
                {
                    Name = request.Name,
                    MasterId = executionContext.UserId
                };

                uow.Groups.Add(group);
                await uow.SaveChangesAsync();

                return group;
            }
        }

        public async Task<List<Group>> GetGroupListAsync(NaheulbookExecutionContext executionContext)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Groups.GetGroupsOwnedByAsync(executionContext.UserId);
            }
        }

        public async Task<Group> GetGroupDetailsAsync(NaheulbookExecutionContext executionContext, int groupId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetGroupsWithDetailsAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                return group;
            }
        }

        public async Task EditGroupPropertiesAsync(NaheulbookExecutionContext executionContext, int groupId, PatchGroupRequest request)
        {
            var notificationSession = _notificationSessionFactory.CreateSession();

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                _groupUtil.ApplyChangesAndNotify(group, request, notificationSession);

                await uow.SaveChangesAsync();
            }

            await notificationSession.CommitAsync();
        }

        public async Task<List<GroupHistoryEntry>> GetGroupHistoryEntriesAsync(NaheulbookExecutionContext executionContext, int groupId, int page)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetGroupsWithDetailsAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                return await uow.GroupHistoryEntries.GetByGroupIdAndPageAsync(groupId, page);
            }
        }

        public async Task EnsureUserCanAccessGroupAsync(NaheulbookExecutionContext executionContext, int groupId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetGroupsWithCharactersAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwnerOrMember(executionContext, group);
            }
        }

        public async Task<GroupInvite> CreateInviteAsync(NaheulbookExecutionContext executionContext, int groupId, CreateInviteRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                var character = await uow.Characters.GetWithOriginWithJobsAsync(request.CharacterId);
                if (character == null)
                    throw new CharacterNotFoundException(request.CharacterId);

                if (character.GroupId != null)
                    throw new CharacterAlreadyInAGroupException(request.CharacterId);

                if (request.FromGroup)
                    _authorizationUtil.EnsureIsGroupOwner(executionContext, group);
                else
                    _authorizationUtil.EnsureIsCharacterOwner(executionContext, character);

                var groupInvite = new GroupInvite
                {
                    Character = character,
                    Group = group,
                    FromGroup = request.FromGroup,
                };

                uow.GroupInvites.Add(groupInvite);

                await uow.SaveChangesAsync();

                var session = _notificationSessionFactory.CreateSession();
                session.NotifyCharacterGroupInvite(request.CharacterId, groupInvite);
                session.NotifyGroupCharacterInvite(groupId, groupInvite);
                await session.CommitAsync();

                return groupInvite;
            }
        }

        public async Task<GroupInvite> CancelOrRejectInviteAsync(NaheulbookExecutionContext executionContext, int groupId, int characterId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var groupInvite = await uow.GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId);
                if (groupInvite == null)
                    throw new InviteNotFoundException(characterId, groupId);

                _authorizationUtil.EnsureCanDeleteGroupInvite(executionContext, groupInvite);

                uow.GroupInvites.Remove(groupInvite);

                await uow.SaveChangesAsync();

                var session = _notificationSessionFactory.CreateSession();
                session.NotifyCharacterCancelGroupInvite(characterId, groupInvite);
                session.NotifyGroupCancelGroupInvite(groupId, groupInvite);
                await session.CommitAsync();

                return groupInvite;
            }
        }

        public async Task AcceptInviteAsync(NaheulbookExecutionContext executionContext, int groupId, int characterId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var groupInvite = await uow.GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId);
                if (groupInvite == null)
                    throw new InviteNotFoundException(characterId, groupId);
                if (groupInvite.Character.GroupId.HasValue)
                    throw new CharacterAlreadyInAGroupException(characterId);

                _authorizationUtil.EnsureCanAcceptGroupInvite(executionContext, groupInvite);

                var allCharacterInvites = await uow.GroupInvites.GetInvitesByCharacterIdAsync(characterId);
                uow.GroupInvites.RemoveRange(allCharacterInvites);
                groupInvite.Character.GroupId = groupInvite.GroupId;

                await uow.SaveChangesAsync();

                var notificationSession = _notificationSessionFactory.CreateSession();
                notificationSession.NotifyCharacterAcceptGroupInvite(characterId, groupInvite);
                notificationSession.NotifyGroupAcceptGroupInvite(groupId, groupInvite);
                await notificationSession.CommitAsync();
            }
        }

        public async Task StartCombatAsync(NaheulbookExecutionContext executionContext, int groupId)
        {
            var notificationSession = _notificationSessionFactory.CreateSession();

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                _groupUtil.StartCombat(group, notificationSession);

                await uow.SaveChangesAsync();

                if (group.CombatLoot != null)
                    notificationSession.NotifyGroupAddLoot(group.Id, group.CombatLoot);
            }

            await notificationSession.CommitAsync();
        }

        public async Task EndCombatAsync(NaheulbookExecutionContext executionContext, int groupId)
        {
            var notificationSession = _notificationSessionFactory.CreateSession();

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                _groupUtil.EndCombat(group, notificationSession);

                await uow.SaveChangesAsync();
            }

            await notificationSession.CommitAsync();
        }

        public async Task UpdateDurationsAsync(NaheulbookExecutionContext executionContext, int groupId, IList<PostGroupUpdateDurationsRequest> request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);
            }

            await _durationUtil.UpdateDurationAsync(groupId, _mapper.Map<IList<FighterDurationChanges>>(request));
        }

        public async Task<IEnumerable<Character>> ListActiveCharactersAsync(NaheulbookExecutionContext executionContext, int groupId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetGroupsWithCharactersAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwnerOrMember(executionContext, group);

                return group.Characters.Where(x => x.IsActive);
            }
        }

        public async Task<NhbkDate> AddTimeAsync(NaheulbookExecutionContext executionContext, int groupId, NhbkDateOffset request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                var notificationSession = _notificationSessionFactory.CreateSession();

                var newDate = _groupUtil.AddTimeAndNotify(group, request, notificationSession);

                await uow.SaveChangesAsync();
                await notificationSession.CommitAsync();

                return newDate;
            }
        }

        public async Task AddHistoryEntryAsync(NaheulbookExecutionContext executionContext, int groupId, PostCreateGroupHistoryEntryRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                group.AddHistoryEntry(_groupHistoryUtil.CreateLogEventRp(group, request.IsGm, request.Info));
                await uow.SaveChangesAsync();
            }
        }

        public async Task EditGroupConfigAsync(NaheulbookExecutionContext executionContext, int groupId, PatchGroupConfigRequest request)
        {
            var notificationSession = _notificationSessionFactory.CreateSession();

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                _groupConfigUtil.ApplyChangesAndNotify(group, request, notificationSession);

                await uow.SaveChangesAsync();
            }

            await notificationSession.CommitAsync();
        }
    }
}