using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Group;

public interface IGroupService
{
    Task<GroupEntity> CreateGroupAsync(NaheulbookExecutionContext executionContext, CreateGroupRequest request, CancellationToken cancellationToken = default);
    Task<List<GroupEntity>> GetGroupListAsync(NaheulbookExecutionContext executionContext, CancellationToken cancellationToken = default);
    Task<GroupEntity> GetGroupDetailsAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default);
    Task EditGroupPropertiesAsync(NaheulbookExecutionContext executionContext, int groupId, PatchGroupRequest request, CancellationToken cancellationToken = default);
    Task<List<GroupHistoryEntryEntity>> GetGroupHistoryEntriesAsync(NaheulbookExecutionContext executionContext, int groupId, int page, CancellationToken cancellationToken = default);
    Task EnsureUserCanAccessGroupAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default);
    Task<GroupInviteEntity> CreateInviteAsync(NaheulbookExecutionContext executionContext, int groupId, CreateInviteRequest request, CancellationToken cancellationToken = default);
    Task<GroupInviteEntity> CancelOrRejectInviteAsync(NaheulbookExecutionContext executionContext, int groupId, int characterId, CancellationToken cancellationToken = default);
    Task AcceptInviteAsync(NaheulbookExecutionContext executionContext, int groupId, int characterId, CancellationToken cancellationToken = default);
    Task StartCombatAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default);
    Task EndCombatAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default);
    Task UpdateDurationsAsync(NaheulbookExecutionContext executionContext, int groupId, IList<PostGroupUpdateDurationsRequest> request, CancellationToken cancellationToken = default);
    Task<IEnumerable<CharacterEntity>> ListActiveCharactersAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default);
    Task<NhbkDate> AddTimeAsync(NaheulbookExecutionContext executionContext, int groupId, NhbkDateOffset request, CancellationToken cancellationToken = default);
    Task AddHistoryEntryAsync(NaheulbookExecutionContext executionContext, int groupId, PostCreateGroupHistoryEntryRequest request, CancellationToken cancellationToken = default);
    Task EditGroupConfigAsync(NaheulbookExecutionContext executionContext, int groupId, PatchGroupConfigRequest request, CancellationToken cancellationToken = default);
}

public class GroupService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory,
    IDurationChangeMapper durationChangeMapper,
    IDurationUtil durationUtil,
    IGroupUtil groupUtil,
    IGroupHistoryUtil groupHistoryUtil,
    IGroupConfigUtil groupConfigUtil
) : IGroupService
{
    public async Task<GroupEntity> CreateGroupAsync(NaheulbookExecutionContext executionContext, CreateGroupRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = new GroupEntity
        {
            Name = request.Name,
            MasterId = executionContext.UserId,
            Loots = new List<LootEntity>(),
            Monsters = new List<MonsterEntity>(),
            Characters = new List<CharacterEntity>(),
            Invites = new List<GroupInviteEntity>(),
            Events = new List<EventEntity>(),
            HistoryEntries = new List<GroupHistoryEntryEntity>(),
            Npcs = new List<NpcEntity>(),
        };

        uow.Groups.Add(group);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return group;
    }

    public async Task<List<GroupEntity>> GetGroupListAsync(NaheulbookExecutionContext executionContext, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Groups.GetGroupsOwnedByAsync(executionContext.UserId, cancellationToken);
    }

    public async Task<GroupEntity> GetGroupDetailsAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetGroupsWithDetailsAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        return group;
    }

    public async Task EditGroupPropertiesAsync(NaheulbookExecutionContext executionContext, int groupId, PatchGroupRequest request, CancellationToken cancellationToken = default)
    {
        var notificationSession = notificationSessionFactory.CreateSession();

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        groupUtil.ApplyChangesAndNotify(group, request, notificationSession);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        await notificationSession.CommitAsync();
    }

    public async Task<List<GroupHistoryEntryEntity>> GetGroupHistoryEntriesAsync(NaheulbookExecutionContext executionContext, int groupId, int page, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetGroupsWithDetailsAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        return await uow.GroupHistoryEntries.GetByGroupIdAndPageAsync(groupId, page, cancellationToken);
    }

    public async Task EnsureUserCanAccessGroupAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetGroupsWithCharactersAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwnerOrMember(executionContext, group);
    }

    public async Task<GroupInviteEntity> CreateInviteAsync(NaheulbookExecutionContext executionContext, int groupId, CreateInviteRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        var character = await uow.Characters.GetWithGroupWithJobsWithOriginAsync(request.CharacterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(request.CharacterId);

        if (character.GroupId != null)
            throw new CharacterAlreadyInAGroupException(request.CharacterId);

        if (request.FromGroup)
            authorizationUtil.EnsureIsGroupOwner(executionContext, group);
        else
            authorizationUtil.EnsureIsCharacterOwner(executionContext, character);

        var groupInvite = new GroupInviteEntity
        {
            Character = character,
            Group = group,
            FromGroup = request.FromGroup,
        };

        uow.GroupInvites.Add(groupInvite);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterGroupInvite(request.CharacterId, groupInvite);
        session.NotifyGroupCharacterInvite(groupId, groupInvite);
        await session.CommitAsync();

        return groupInvite;
    }

    public async Task<GroupInviteEntity> CancelOrRejectInviteAsync(NaheulbookExecutionContext executionContext, int groupId, int characterId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var groupInvite = await uow.GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId, cancellationToken);
        if (groupInvite == null)
            throw new InviteNotFoundException(characterId, groupId);

        authorizationUtil.EnsureCanDeleteGroupInvite(executionContext, groupInvite);

        uow.GroupInvites.Remove(groupInvite);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterCancelGroupInvite(characterId, groupInvite);
        session.NotifyGroupCancelGroupInvite(groupId, groupInvite);
        await session.CommitAsync();

        return groupInvite;
    }

    public async Task AcceptInviteAsync(NaheulbookExecutionContext executionContext, int groupId, int characterId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var groupInvite = await uow.GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId, cancellationToken);
        if (groupInvite == null)
            throw new InviteNotFoundException(characterId, groupId);
        if (groupInvite.Character.GroupId.HasValue)
            throw new CharacterAlreadyInAGroupException(characterId);

        authorizationUtil.EnsureCanAcceptGroupInvite(executionContext, groupInvite);

        var allCharacterInvites = await uow.GroupInvites.GetInvitesByCharacterIdAsync(characterId, cancellationToken);
        uow.GroupInvites.RemoveRange(allCharacterInvites);
        groupInvite.Character.GroupId = groupInvite.GroupId;

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterAcceptGroupInvite(characterId, groupInvite);
        notificationSession.NotifyGroupAcceptGroupInvite(groupId, groupInvite);
        await notificationSession.CommitAsync();
    }

    public async Task StartCombatAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default)
    {
        var notificationSession = notificationSessionFactory.CreateSession();

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var group = await uow.Groups.GetAsync(groupId, cancellationToken);
            if (group == null)
                throw new GroupNotFoundException(groupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, group);

            groupUtil.StartCombat(group, notificationSession);

            await uow.SaveChangesAsync(DoNotInterrupt.Token);

            if (group.CombatLoot != null)
                notificationSession.NotifyGroupAddLoot(group.Id, group.CombatLoot);
        }

        await notificationSession.CommitAsync();
    }

    public async Task EndCombatAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default)
    {
        var notificationSession = notificationSessionFactory.CreateSession();

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var group = await uow.Groups.GetAsync(groupId, cancellationToken);
            if (group == null)
                throw new GroupNotFoundException(groupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, group);

            groupUtil.EndCombat(group, notificationSession);

            await uow.SaveChangesAsync(DoNotInterrupt.Token);
        }

        await notificationSession.CommitAsync();
    }

    public async Task UpdateDurationsAsync(NaheulbookExecutionContext executionContext, int groupId, IList<PostGroupUpdateDurationsRequest> request, CancellationToken cancellationToken = default)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var group = await uow.Groups.GetAsync(groupId, cancellationToken);
            if (group == null)
                throw new GroupNotFoundException(groupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, group);
        }

        await durationUtil.UpdateDurationAsync(groupId, request.Select(durationChangeMapper.ToFighterDurationChanges).ToList(), cancellationToken);
    }

    public async Task<IEnumerable<CharacterEntity>> ListActiveCharactersAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetGroupsWithCharactersAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwnerOrMember(executionContext, group);

        return group.Characters.Where(x => x.IsActive);
    }

    public async Task<NhbkDate> AddTimeAsync(NaheulbookExecutionContext executionContext, int groupId, NhbkDateOffset request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var notificationSession = notificationSessionFactory.CreateSession();

        var newDate = groupUtil.AddTimeAndNotify(group, request, notificationSession);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();

        return newDate;
    }

    public async Task AddHistoryEntryAsync(NaheulbookExecutionContext executionContext, int groupId, PostCreateGroupHistoryEntryRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        group.AddHistoryEntry(groupHistoryUtil.CreateLogEventRp(group, request.IsGm, request.Info));
        await uow.SaveChangesAsync(DoNotInterrupt.Token);
    }

    public async Task EditGroupConfigAsync(NaheulbookExecutionContext executionContext, int groupId, PatchGroupConfigRequest request, CancellationToken cancellationToken = default)
    {
        var notificationSession = notificationSessionFactory.CreateSession();

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var group = await uow.Groups.GetAsync(groupId, cancellationToken);
            if (group == null)
                throw new GroupNotFoundException(groupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, group);

            groupConfigUtil.ApplyChangesAndNotify(group, request, notificationSession);

            await uow.SaveChangesAsync(DoNotInterrupt.Token);
        }

        await notificationSession.CommitAsync();
    }
}