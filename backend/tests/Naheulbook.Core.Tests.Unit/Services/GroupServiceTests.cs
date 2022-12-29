using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.Exceptions;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services;

public class GroupServiceTests
{
    private FakeUnitOfWorkFactory _unitOfWorkFactory;
    private IAuthorizationUtil _authorizationUtil;
    private FakeNotificationSessionFactory _notificationSessionFactory;
    private IMapper _mapper;
    private IGroupUtil _groupUtil;
    private GroupService _service;
    private IDurationUtil _durationUtil;
    private IGroupHistoryUtil _groupHistoryUtil;
    private IGroupConfigUtil _groupConfigUtil;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkFactory = new FakeUnitOfWorkFactory();
        _authorizationUtil = Substitute.For<IAuthorizationUtil>();
        _notificationSessionFactory = new FakeNotificationSessionFactory();
        _mapper = Substitute.For<IMapper>();
        _groupUtil = Substitute.For<IGroupUtil>();
        _durationUtil = Substitute.For<IDurationUtil>();
        _groupHistoryUtil = Substitute.For<IGroupHistoryUtil>();
        _groupConfigUtil = Substitute.For<IGroupConfigUtil>();

        _service = new GroupService(
            _unitOfWorkFactory,
            _authorizationUtil,
            _notificationSessionFactory,
            _mapper,
            _durationUtil,
            _groupUtil,
            _groupHistoryUtil,
            _groupConfigUtil
        );
    }

    [Test]
    public async Task CreateGroupAsync_CreateANewGroupInDatabase_SettingMasterToCurrentUserId()
    {
        var createGroupRequest = new CreateGroupRequest {Name = "some-name"};
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};

        var actualGroup = await _service.CreateGroupAsync(naheulbookExecutionContext, createGroupRequest);

        Received.InOrder(() =>
        {
            _unitOfWorkFactory.GetUnitOfWork().Groups.Add(actualGroup);
            _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });

        actualGroup.Name.Should().Be("some-name");
        actualGroup.MasterId.Should().Be(10);
    }

    [Test]
    public async Task GetGroupDetailsAsync_ShouldLoadGroupDetailsAndReturnIt()
    {
        const int groupId = 4;
        var naheulbookExecutionContext = new NaheulbookExecutionContext();
        var group = new GroupEntity();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetGroupsWithDetailsAsync(groupId)
            .Returns(group);

        var actualGroup = await _service.GetGroupDetailsAsync(naheulbookExecutionContext, groupId);

        actualGroup.Should().BeSameAs(group);
    }


    [Test]
    public async Task GetGroupDetailsAsync_ShouldThrowWhenGroupDoesNotExists()
    {
        const int groupId = 4;
        var naheulbookExecutionContext = new NaheulbookExecutionContext();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetGroupsWithDetailsAsync(groupId)
            .Returns((GroupEntity) null);

        Func<Task> act = () => _service.GetGroupDetailsAsync(naheulbookExecutionContext, groupId);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }


    [Test]
    public async Task GetGroupDetailsAsync_ShouldEnsureIsGroupOwner()
    {
        const int groupId = 4;
        var naheulbookExecutionContext = new NaheulbookExecutionContext();
        var group = new GroupEntity();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetGroupsWithDetailsAsync(groupId)
            .Returns(group);

        _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
            .Throw(new TestException());

        Func<Task> act = () => _service.GetGroupDetailsAsync(naheulbookExecutionContext, groupId);

        await act.Should().ThrowAsync<TestException>();
    }

    [Test]
    public async Task EditGroupPropertiesAsync_ShouldApplyChangeOnGroupThenSave()
    {
        const int groupId = 4;
        var naheulbookExecutionContext = new NaheulbookExecutionContext();
        var request = new PatchGroupRequest();
        var group = new GroupEntity();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);

        await _service.EditGroupPropertiesAsync(naheulbookExecutionContext, groupId, request);

        Received.InOrder(() =>
        {
            _groupUtil.ApplyChangesAndNotify(group, request, _notificationSessionFactory.NotificationSession);
            _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
            _notificationSessionFactory.NotificationSession.CommitAsync();
        });
    }

    [Test]
    public async Task EditGroupPropertiesAsync_ShouldThrowWhenGroupDoesNotExists()
    {
        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(Arg.Any<int>())
            .Returns((GroupEntity) null);

        Func<Task> act = () => _service.EditGroupPropertiesAsync(new NaheulbookExecutionContext(), 4, new PatchGroupRequest());

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Test]
    public async Task EditGroupPropertiesAsync_ShouldEnsureIsGroupOwner()
    {
        var naheulbookExecutionContext = new NaheulbookExecutionContext();
        var group = new GroupEntity();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(Arg.Any<int>())
            .Returns(group);

        _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
            .Throw(new TestException());

        Func<Task> act = () => _service.EditGroupPropertiesAsync(naheulbookExecutionContext, 4, new PatchGroupRequest());

        await act.Should().ThrowAsync<TestException>();
    }

    [Test]
    public async Task CreateInviteAsync_ShouldCreateANewGroupInviteInDatabase()
    {
        const int groupId = 42;
        const int characterId = 24;
        var request = new CreateInviteRequest {CharacterId = characterId, FromGroup = true};
        var group = new GroupEntity();
        var character = new CharacterEntity();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);
        _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
            .Returns(character);

        await _service.CreateInviteAsync(new NaheulbookExecutionContext(), groupId, request);

        Received.InOrder(() =>
        {
            _unitOfWorkFactory.GetUnitOfWork().GroupInvites.Add(Arg.Is<GroupInviteEntity>(gi => gi.FromGroup && gi.Group == group && gi.Character == character));
            _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });
    }

    [Test]
    public async Task CreateInviteAsync_ShouldNotifyChange()
    {
        const int groupId = 42;
        const int characterId = 24;
        var request = new CreateInviteRequest {CharacterId = characterId, FromGroup = true};
        var group = new GroupEntity();
        var character = new CharacterEntity();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);
        _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
            .Returns(character);

        var groupInvite = await _service.CreateInviteAsync(new NaheulbookExecutionContext(), groupId, request);

        Received.InOrder(() =>
        {
            _notificationSessionFactory.NotificationSession.NotifyCharacterGroupInvite(characterId, groupInvite);
            _notificationSessionFactory.NotificationSession.NotifyGroupCharacterInvite(groupId, groupInvite);
            _notificationSessionFactory.NotificationSession.CommitAsync();
        });
    }

    [Test]
    public async Task CreateInviteAsync_ShouldThrowWhenGroupNotFound()
    {
        const int groupId = 42;
        var request = new CreateInviteRequest();
        var executionContext = new NaheulbookExecutionContext();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns((GroupEntity) null);

        Func<Task> act = () => _service.CreateInviteAsync(executionContext, groupId, request);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Test]
    public async Task CreateInviteAsync_ShouldThrowWhenCharacterNotFound()
    {
        const int groupId = 42;
        const int characterId = 24;
        var request = new CreateInviteRequest {CharacterId = characterId};
        var executionContext = new NaheulbookExecutionContext();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(new GroupEntity());
        _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
            .Returns((CharacterEntity) null);

        Func<Task> act = () => _service.CreateInviteAsync(executionContext, groupId, request);

        await act.Should().ThrowAsync<CharacterNotFoundException>();
    }

    [Test]
    public async Task CreateInviteAsync_WhenFromGroup_EnsureUserIsOwnerOfGroup()
    {
        const int groupId = 42;
        const int characterId = 24;
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};
        var request = new CreateInviteRequest {CharacterId = characterId, FromGroup = true};
        var group = new GroupEntity {Id = groupId};
        var character = new CharacterEntity {Id = characterId};

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);
        _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
            .Returns(character);
        _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
            .Throw(new TestException());

        Func<Task> act = () => _service.CreateInviteAsync(naheulbookExecutionContext, groupId, request);

        await act.Should().ThrowAsync<TestException>();
        await _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().SaveChangesAsync();
    }

    [Test]
    public async Task CreateInviteAsync_WhenFromCharacter_EnsureUserIsOwnerOfCharacter()
    {
        const int groupId = 42;
        const int characterId = 24;
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};
        var request = new CreateInviteRequest {CharacterId = characterId, FromGroup = false};
        var group = new GroupEntity {Id = groupId};
        var character = new CharacterEntity {Id = characterId};

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);
        _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
            .Returns(character);
        _authorizationUtil.When(x => x.EnsureIsCharacterOwner(naheulbookExecutionContext, character))
            .Throw(new TestException());

        Func<Task> act = () => _service.CreateInviteAsync(naheulbookExecutionContext, groupId, request);

        await act.Should().ThrowAsync<TestException>();
        await _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().SaveChangesAsync();
    }

    [Test]
    public async Task CancelOrRejectInviteAsync_ShouldDeleteInviteFromDatabase()
    {

        const int groupId = 42;
        const int characterId = 24;
        var executionContext = new NaheulbookExecutionContext();
        var groupInvite = new GroupInviteEntity();

        _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
            .Returns(groupInvite);

        await _service.CancelOrRejectInviteAsync(executionContext, groupId, characterId);

        Received.InOrder(() =>
        {
            _unitOfWorkFactory.GetUnitOfWork().GroupInvites.Remove(groupInvite);
            _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });
    }

    [Test]
    public async Task CancelOrRejectInviteAsync_ShouldNotifyCharacterAndGroupOfThat()
    {

        const int groupId = 42;
        const int characterId = 24;
        var executionContext = new NaheulbookExecutionContext();
        var groupInvite = new GroupInviteEntity();

        _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
            .Returns(groupInvite);

        await _service.CancelOrRejectInviteAsync(executionContext, groupId, characterId);

        Received.InOrder(() =>
        {
            _notificationSessionFactory.NotificationSession.NotifyCharacterCancelGroupInvite(characterId, groupInvite);
            _notificationSessionFactory.NotificationSession.NotifyGroupCancelGroupInvite(groupId, groupInvite);
            _notificationSessionFactory.NotificationSession.CommitAsync();
        });
    }

    [Test]
    public async Task CancelOrRejectInviteAsync_ShouldThrowIfInviteDoesNotExists()
    {
        const int groupId = 42;
        const int characterId = 24;

        _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
            .Returns((GroupInviteEntity) null);

        Func<Task> act = () => _service.CancelOrRejectInviteAsync(new NaheulbookExecutionContext(), groupId, characterId);

        await act.Should().ThrowAsync<InviteNotFoundException>();
    }

    [Test]
    public async Task CancelOrRejectInviteAsync_ShouldEnsureUserCanDeleteGroupInvite()
    {
        const int groupId = 42;
        const int characterId = 24;
        var executionContext = new NaheulbookExecutionContext();
        var groupInvite = new GroupInviteEntity();

        _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
            .Returns(groupInvite);
        _authorizationUtil.When(x => x.EnsureCanDeleteGroupInvite(executionContext, groupInvite))
            .Throw(new TestException());

        Func<Task> act = () => _service.CancelOrRejectInviteAsync(executionContext, groupId, characterId);

        await act.Should().ThrowAsync<TestException>();
    }

    [Test]
    public async Task AcceptInviteAsync_ShouldDeleteInvitesFromDatabase_AndChangeCharacterGroupId()
    {
        const int groupId = 42;
        const int characterId = 24;
        var character = new CharacterEntity();
        var executionContext = new NaheulbookExecutionContext();
        var groupInvite = new GroupInviteEntity {GroupId = groupId, Character = character};
        var otherGroupInvite = new GroupInviteEntity {GroupId = groupId, Character = character};
        var groupInvites = new List<GroupInviteEntity> {groupInvite, otherGroupInvite};

        _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
            .Returns(groupInvite);
        _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetInvitesByCharacterIdAsync(characterId)
            .Returns(groupInvites);
        _unitOfWorkFactory.GetUnitOfWork().When(x => x.SaveChangesAsync())
            .Do(_ => character.GroupId.Should().Be(groupId));

        await _service.AcceptInviteAsync(executionContext, groupId, characterId);

        Received.InOrder(() =>
        {
            _unitOfWorkFactory.GetUnitOfWork().GroupInvites.RemoveRange(groupInvites);
            _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });
    }

    [Test]
    public async Task AcceptInviteAsync_ShouldNotifyCharacterAndGroupOfThat()
    {

        const int groupId = 42;
        const int characterId = 24;
        var character = new CharacterEntity();
        var executionContext = new NaheulbookExecutionContext();
        var groupInvite = new GroupInviteEntity {GroupId = groupId, Character = character};

        _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
            .Returns(groupInvite);

        await _service.AcceptInviteAsync(executionContext, groupId, characterId);

        Received.InOrder(() =>
        {
            _notificationSessionFactory.NotificationSession.NotifyCharacterAcceptGroupInvite(characterId, groupInvite);
            _notificationSessionFactory.NotificationSession.NotifyGroupAcceptGroupInvite(groupId, groupInvite);
            _notificationSessionFactory.NotificationSession.CommitAsync();
        });
    }

    [Test]
    public async Task AcceptInviteAsync_ShouldThrowIfInviteDoesNotExists()
    {
        const int groupId = 42;
        const int characterId = 24;

        _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
            .Returns((GroupInviteEntity) null);

        Func<Task> act = () => _service.AcceptInviteAsync(new NaheulbookExecutionContext(), groupId, characterId);

        await act.Should().ThrowAsync<InviteNotFoundException>();
    }

    [Test]
    public async Task AcceptInviteAsync_ShouldThrowIfCharacterIsAlreadyInAGroup()
    {
        const int groupId = 42;
        const int characterId = 24;
        var character = new CharacterEntity {GroupId = 8};
        var groupInvite = new GroupInviteEntity {GroupId = groupId, Character = character};

        _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
            .Returns(groupInvite);

        Func<Task> act = () => _service.AcceptInviteAsync(new NaheulbookExecutionContext(), groupId, characterId);

        await act.Should().ThrowAsync<CharacterAlreadyInAGroupException>();
    }

    [Test]
    public async Task AcceptInviteAsync_ShouldEnsureUserCanAcceptGroupInvite()
    {
        const int groupId = 42;
        const int characterId = 24;
        var executionContext = new NaheulbookExecutionContext();
        var groupInvite = new GroupInviteEntity {Character = new CharacterEntity()};

        _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
            .Returns(groupInvite);
        _authorizationUtil.When(x => x.EnsureCanAcceptGroupInvite(executionContext, groupInvite))
            .Throw(new TestException());

        Func<Task> act = () => _service.AcceptInviteAsync(executionContext, groupId, characterId);

        await act.Should().ThrowAsync<TestException>();
    }

    [Test]
    public async Task UpdateDurationsAsync_ShouldCallGroupUtilUpdateDurationAsync()
    {
        const int groupId = 42;
        var group = new GroupEntity();
        var fighters = new List<FighterDurationChanges>();
        var request = new List<PostGroupUpdateDurationsRequest>();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);
        _mapper.Map<IList<FighterDurationChanges>>(request)
            .Returns(fighters);

        await _service.UpdateDurationsAsync(new NaheulbookExecutionContext(), groupId, request);

        await _durationUtil.Received(1).UpdateDurationAsync(groupId, fighters);
    }

    [Test]
    public async Task AcceptInviteAsync_ShouldThrowIfGroupDoesNotExists()
    {
        const int groupId = 42;

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns((GroupEntity) null);

        Func<Task> act = () => _service.UpdateDurationsAsync(new NaheulbookExecutionContext(), groupId, new List<PostGroupUpdateDurationsRequest>());

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Test]
    public async Task UpdateDurationsAsync_ShouldEnsureUserIsGroupMaster()
    {
        const int groupId = 42;
        var group = new GroupEntity();
        var executionContext = new NaheulbookExecutionContext();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(Arg.Any<int>())
            .Returns(group);
        _authorizationUtil.When(x => x.EnsureIsGroupOwner(executionContext, group))
            .Throw(new TestException());

        Func<Task> act = () => _service.UpdateDurationsAsync(executionContext, groupId, new List<PostGroupUpdateDurationsRequest>());

        await act.Should().ThrowAsync<TestException>();
    }
}