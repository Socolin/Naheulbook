using System;
using System.Collections.Generic;
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
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class MonsterServiceTests
    {
        private FakeUnitOfWorkFactory _unitOfWorkFactory;
        private IAuthorizationUtil _authorizationUtil;
        private IActiveStatsModifierUtil _activeStatsModifierUtil;
        private FakeNotificationSessionFactory _notificationSessionFactory;
        private IJsonUtil _jsonUtil;
        private MonsterService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _activeStatsModifierUtil = Substitute.For<IActiveStatsModifierUtil>();
            _notificationSessionFactory = new FakeNotificationSessionFactory();
            _jsonUtil = Substitute.For<IJsonUtil>();

            _service = new MonsterService(
                _unitOfWorkFactory,
                _authorizationUtil,
                _activeStatsModifierUtil,
                _notificationSessionFactory,
                _jsonUtil
            );
        }

        [Test]
        public async Task CreateMonsterAsync_ShouldCreateNewEntryInDatabase_AndLinkItToGroup()
        {
            const int groupId = 42;
            var request = CreateRequest();
            var executionContext = new NaheulbookExecutionContext();
            var group = new Group {Id = groupId};

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);

            var actualMonster = await _service.CreateMonsterAsync(executionContext, groupId, request);

            Received.InOrder(() =>
            {
                _activeStatsModifierUtil.InitializeModifierIds(request.Modifiers);
                _unitOfWorkFactory.GetUnitOfWork().Monsters.Add(actualMonster);
                _unitOfWorkFactory.GetUnitOfWork().CompleteAsync();
            });

            actualMonster.Should().BeEquivalentTo(new Monster
            {
                Name = "some-monster-name",
                Data = @"{""key"":""value""}",
                Modifiers = "[]",
                Group = group
            });
        }

        [Test]
        public void CreateMonsterAsync_ShouldThrowWhenGroupNotFound()
        {
            const int groupId = 42;
            var request = CreateRequest();
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns((Group) null);

            Func<Task> act = () => _service.CreateMonsterAsync(executionContext, groupId, request);

            act.Should().Throw<GroupNotFoundException>();
        }

        [Test]
        public void CreateMonsterAsync_EnsureUserIsOwnerOfGroup()
        {
            const int groupId = 42;
            var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};
            var group = new Group {Id = groupId};

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);

            _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
                .Throw(new TestException());

            Func<Task> act = () => _service.CreateMonsterAsync(naheulbookExecutionContext, groupId, new CreateMonsterRequest());

            act.Should().Throw<TestException>();
        }


        [Test]
        public async Task GetMonstersForGroupAsync_ShouldLoadMonstersListAndReturnIt()
        {
            const int groupId = 42;
            var executionContext = new NaheulbookExecutionContext();
            var group = new Group {Id = groupId};
            var expectedMonsters = new List<Monster>();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);
            _unitOfWorkFactory.GetUnitOfWork().Monsters.GetByGroupIdWithInventoryAsync(groupId)
                .Returns(expectedMonsters);

            var events = await _service.GetMonstersForGroupAsync(executionContext, groupId);

            events.Should().BeSameAs(expectedMonsters);
        }

        [Test]
        public void GetMonstersForGroupAsync_ShouldThrowWhenGroupNotFound()
        {
            const int groupId = 42;
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns((Group) null);

            Func<Task> act = () => _service.GetMonstersForGroupAsync(executionContext, groupId);

            act.Should().Throw<GroupNotFoundException>();
        }

        [Test]
        public void GetMonstersForGroupAsync_ShouldEnsureGroupAccess()
        {
            const int groupId = 42;
            var naheulbookExecutionContext = new NaheulbookExecutionContext();
            var group = new Group {Id = groupId};

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);

            _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
                .Throw(new TestException());

            Func<Task> act = () => _service.GetMonstersForGroupAsync(naheulbookExecutionContext, groupId);

            act.Should().Throw<TestException>();
        }

        private static CreateMonsterRequest CreateRequest()
        {
            return new CreateMonsterRequest
            {
                Name = "some-monster-name",
                Data = JObject.FromObject(new {key = "value"}),
                Items = new List<object>(),
                Modifiers = new List<ActiveStatsModifier>()
            };
        }
    }
}