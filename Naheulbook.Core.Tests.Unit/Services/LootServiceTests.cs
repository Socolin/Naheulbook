using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
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
    public class LootServiceTests
    {
        private FakeUnitOfWorkFactory _unitOfWorkFactory;
        private IAuthorizationUtil _authorizationUtil;
        private INotificationSessionFactory _notificationSessionFactory;
        private IItemService _itemService;
        private LootService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _notificationSessionFactory = Substitute.For<INotificationSessionFactory>();
            _itemService = Substitute.For<IItemService>();

            _service = new LootService(
                _unitOfWorkFactory,
                _authorizationUtil,
                _notificationSessionFactory,
                _itemService
            );
        }

        [Test]
        public async Task CreateLootAsync_CreateALootInDb_AndReturnIt()
        {
            const int groupId = 42;
            var createLootRequest = new CreateLootRequest {Name = "some-name"};
            var naheulbookExecutionContext = new NaheulbookExecutionContext();
            var group = new Group {Id = groupId};

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);

            var actualLoot = await _service.CreateLootAsync(naheulbookExecutionContext, groupId, createLootRequest);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().Loots.Add(actualLoot);
                _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
            });

            actualLoot.Name.Should().Be("some-name");
            actualLoot.Group.Should().BeSameAs(group);
        }

        [Test]
        public void CreateLootAsync_WhenGroupDoesNotExists_ShouldThrow()
        {
            var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(42)
                .Returns((Group) null);

            Func<Task> act = () => _service.CreateLootAsync(naheulbookExecutionContext, 42, new CreateLootRequest());

            act.Should().Throw<GroupNotFoundException>();
        }

        [Test]
        public void CreateLootAsync_EnsureUserAccessToLoot()
        {
            const int groupId = 42;
            var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};
            var group = new Group {Id = groupId};

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);

            _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
                .Throw(new TestException());

            Func<Task> act = () => _service.CreateLootAsync(naheulbookExecutionContext, groupId, new CreateLootRequest());

            act.Should().Throw<TestException>();
        }

        [Test]
        public async Task GetLootsForGroupAsync_ShouldLoadLootsListAndReturnIt()
        {
            const int groupId = 42;
            var executionContext = new NaheulbookExecutionContext();
            var group = new Group {Id = groupId};
            var expectedLoots = new List<Loot>();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);
            _unitOfWorkFactory.GetUnitOfWork().Loots.GetByGroupIdAsync(groupId)
                .Returns(expectedLoots);

            var loots = await _service.GetLootsForGroupAsync(executionContext, groupId);

            loots.Should().BeSameAs(expectedLoots);
        }

        [Test]
        public void GetLootsForGroupAsync_ShouldThrowWhenGroupNotFound()
        {
            const int groupId = 42;
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns((Group) null);

            Func<Task> act = () => _service.GetLootsForGroupAsync(executionContext, groupId);

            act.Should().Throw<GroupNotFoundException>();
        }

        [Test]
        public void GetLootsForGroupAsync_ShouldEnsureGroupAccess()
        {
            const int groupId = 42;
            var naheulbookExecutionContext = new NaheulbookExecutionContext();
            var group = new Group {Id = groupId};

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);

            _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
                .Throw(new TestException());

            Func<Task> act = () => _service.GetLootsForGroupAsync(naheulbookExecutionContext, groupId);

            act.Should().Throw<TestException>();
        }
    }
}