using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface IGroupService
    {
        Task<Group> CreateGroupAsync(NaheulbookExecutionContext executionContext, CreateGroupRequest request);
        Task<List<Group>> GetGroupListAsync(NaheulbookExecutionContext executionContext);
        Task<Group> GetGroupDetailsAsync(NaheulbookExecutionContext executionContext, int groupId);
        Task<List<GroupHistoryEntry>> GetGroupHistoryEntriesAsync(NaheulbookExecutionContext executionContext, int groupId, int page);
        Task EnsureUserCanAccessGroupAsync(NaheulbookExecutionContext executionContext, int groupId);
        Task CreateInviteAsync(NaheulbookExecutionContext executionContext, int groupId, CreateInviteRequest request);
    }

    public class GroupService : IGroupService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;

        public GroupService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
        }

        public async Task<Group> CreateGroupAsync(NaheulbookExecutionContext executionContext, CreateGroupRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var location = await uow.Locations.GetNewGroupDefaultLocationAsync();

                var group = new Group
                {
                    Name = request.Name,
                    MasterId = executionContext.UserId,
                    Location = location
                };

                uow.Groups.Add(group);
                await uow.CompleteAsync();

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

        public async Task CreateInviteAsync(NaheulbookExecutionContext executionContext, int groupId, CreateInviteRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                var character = await uow.Characters.GetAsync(request.CharacterId);
                if (character == null)
                    throw new CharacterNotFoundException(request.CharacterId);

                if (request.FromGroup)
                    _authorizationUtil.EnsureIsGroupOwner(executionContext, group);
                else
                    _authorizationUtil.EnsureIsCharacterOwner(executionContext, character);

                uow.GroupInvites.Add(new GroupInvite
                {
                    Character = character,
                    Group = group,
                    FromGroup = request.FromGroup,
                });

                await uow.CompleteAsync();
            }
        }
    }
}