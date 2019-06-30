using System;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.Exceptions;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class GroupServiceTests
    {
        private FakeUnitOfWorkFactory _unitOfWorkFactory;
        private IAuthorizationUtil _authorizationUtil;
        private IChangeNotifier _changeNotifier;
        private GroupService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _changeNotifier = Substitute.For<IChangeNotifier>();

            _service = new GroupService(
                _unitOfWorkFactory,
                _authorizationUtil,
                _changeNotifier
            );
        }

        [Test]
        public async Task CreateGroupAsync_CreateANewGroupInDatabase_SettingMasterToCurrentUserId()
        {
            var createGroupRequest = new CreateGroupRequest {Name = "some-name"};
            var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};
            var location = new Location();

            _unitOfWorkFactory.GetUnitOfWork().Locations.GetNewGroupDefaultLocationAsync()
                .Returns(location);

            var actualGroup = await _service.CreateGroupAsync(naheulbookExecutionContext, createGroupRequest);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().Groups.Add(actualGroup);
                _unitOfWorkFactory.GetUnitOfWork().CompleteAsync();
            });

            actualGroup.Location.Should().BeSameAs(location);
            actualGroup.Name.Should().Be("some-name");
            actualGroup.MasterId.Should().Be(10);
        }

        [Test]
        public async Task GetGroupDetailsAsync_ShouldLoadGroupDetailsAndReturnIt()
        {
            const int groupId = 4;
            var naheulbookExecutionContext = new NaheulbookExecutionContext();
            var group = new Group();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetGroupsWithDetailsAsync(groupId)
                .Returns(group);

            var actualGroup = await _service.GetGroupDetailsAsync(naheulbookExecutionContext, groupId);

            actualGroup.Should().BeSameAs(group);
        }


        [Test]
        public void GetGroupDetailsAsync_ShouldThrowWhenGroupDoesNotExists()
        {
            const int groupId = 4;
            var naheulbookExecutionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetGroupsWithDetailsAsync(groupId)
                .Returns((Group) null);

            Func<Task> act = () => _service.GetGroupDetailsAsync(naheulbookExecutionContext, groupId);

            act.Should().Throw<GroupNotFoundException>();
        }


        [Test]
        public void GetGroupDetailsAsync_ShouldEnsureIsGroupOwner()
        {
            const int groupId = 4;
            var naheulbookExecutionContext = new NaheulbookExecutionContext();
            var group = new Group();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetGroupsWithDetailsAsync(groupId)
                .Returns(group);

            _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
                .Throw(new TestException());

            Func<Task> act = () => _service.GetGroupDetailsAsync(naheulbookExecutionContext, groupId);

            act.Should().Throw<TestException>();
        }

        [Test]
        public async Task CreateInviteAsync_ShouldCreateANewGroupInviteInDatabase()
        {
            const int groupId = 42;
            const int characterId = 24;
            var request = new CreateInviteRequest {CharacterId = characterId, FromGroup = true};
            var group = new Group();
            var character = new Character();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);
            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
                .Returns(character);

            await _service.CreateInviteAsync(new NaheulbookExecutionContext(), groupId, request);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().GroupInvites.Add(Arg.Is<GroupInvite>(gi => gi.FromGroup && gi.Group == group && gi.Character == character));
                _unitOfWorkFactory.GetUnitOfWork().CompleteAsync();
            });
        }

        [Test]
        public async Task CreateInviteAsync_ShouldNotifyChange()
        {
            const int groupId = 42;
            const int characterId = 24;
            var request = new CreateInviteRequest {CharacterId = characterId, FromGroup = true};
            var group = new Group();
            var character = new Character();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);
            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
                .Returns(character);

            var groupInvite = await _service.CreateInviteAsync(new NaheulbookExecutionContext(), groupId, request);

            await _changeNotifier.Received(1).NotifyCharacterGroupInviteAsync(characterId, groupInvite);
            await _changeNotifier.Received(1).NotifyGroupCharacterInviteAsync(groupId, groupInvite);
        }

        [Test]
        public void CreateInviteAsync_ShouldThrowWhenGroupNotFound()
        {
            const int groupId = 42;
            var request = new CreateInviteRequest();
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns((Group) null);

            Func<Task> act = () => _service.CreateInviteAsync(executionContext, groupId, request);

            act.Should().Throw<GroupNotFoundException>();
        }

        [Test]
        public void CreateInviteAsync_ShouldThrowWhenCharacterNotFound()
        {
            const int groupId = 42;
            const int characterId = 24;
            var request = new CreateInviteRequest {CharacterId = characterId};
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(new Group());
            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
                .Returns((Character) null);

            Func<Task> act = () => _service.CreateInviteAsync(executionContext, groupId, request);

            act.Should().Throw<CharacterNotFoundException>();
        }

        [Test]
        public void CreateInviteAsync_WhenFromGroup_EnsureUserIsOwnerOfGroup()
        {
            const int groupId = 42;
            const int characterId = 24;
            var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};
            var request = new CreateInviteRequest {CharacterId = characterId, FromGroup = true};
            var group = new Group {Id = groupId};
            var character = new Character {Id = characterId};

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);
            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
                .Returns(character);
            _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
                .Throw(new TestException());

            Func<Task> act = () => _service.CreateInviteAsync(naheulbookExecutionContext, groupId, request);

            act.Should().Throw<TestException>();
            _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().CompleteAsync();
        }

        [Test]
        public void CreateInviteAsync_WhenFromCharacter_EnsureUserIsOwnerOfCharacter()
        {
            const int groupId = 42;
            const int characterId = 24;
            var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};
            var request = new CreateInviteRequest {CharacterId = characterId, FromGroup = false};
            var group = new Group {Id = groupId};
            var character = new Character {Id = characterId};

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);
            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
                .Returns(character);
            _authorizationUtil.When(x => x.EnsureIsCharacterOwner(naheulbookExecutionContext, character))
                .Throw(new TestException());

            Func<Task> act = () => _service.CreateInviteAsync(naheulbookExecutionContext, groupId, request);

            act.Should().Throw<TestException>();
            _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().CompleteAsync();
        }

        [Test]
        public async Task CancelOrRejectInviteAsync_ShouldDeleteInviteFromDatabase()
        {

            const int groupId = 42;
            const int characterId = 24;
            var executionContext = new NaheulbookExecutionContext();
            var groupInvite = new GroupInvite();

            _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
                .Returns(groupInvite);

            await _service.CancelOrRejectInviteAsync(executionContext, groupId, characterId);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().GroupInvites.Remove(groupInvite);
                _unitOfWorkFactory.GetUnitOfWork().CompleteAsync();
            });
        }

        [Test]
        public async Task CancelOrRejectInviteAsync_ShouldNotifyCharacterAndGroupOfThat()
        {

            const int groupId = 42;
            const int characterId = 24;
            var executionContext = new NaheulbookExecutionContext();
            var groupInvite = new GroupInvite();

            _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
                .Returns(groupInvite);

            await _service.CancelOrRejectInviteAsync(executionContext, groupId, characterId);

            await _changeNotifier.Received(1).NotifyGroupCancelGroupInviteAsync(groupId, groupInvite);
            await _changeNotifier.Received(1).NotifyCharacterCancelGroupInviteAsync(characterId, groupInvite);
        }

        [Test]
        public void CancelOrRejectInviteAsync_ShouldThrowIfInviteDoesNotExists()
        {
            const int groupId = 42;
            const int characterId = 24;

            _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
                .Returns((GroupInvite) null);

            Func<Task> act = () => _service.CancelOrRejectInviteAsync(new NaheulbookExecutionContext(), groupId, characterId);

            act.Should().Throw<InviteNotFoundException>();
        }

        [Test]
        public void CancelOrRejectInviteAsync_ShouldEnsureUserCanDeleteThisException()
        {
            const int groupId = 42;
            const int characterId = 24;
            var executionContext = new NaheulbookExecutionContext();
            var groupInvite = new GroupInvite();

            _unitOfWorkFactory.GetUnitOfWork().GroupInvites.GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(groupId, characterId)
                .Returns(groupInvite);
            _authorizationUtil.When(x => x.EnsureCanDeleteGroupInvite(executionContext, groupInvite))
                .Throw(new TestException());

            Func<Task> act = () => _service.CancelOrRejectInviteAsync(executionContext, groupId, characterId);

            act.Should().Throw<TestException>();
        }
    }
}