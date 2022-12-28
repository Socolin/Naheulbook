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

namespace Naheulbook.Core.Tests.Unit.Services;

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
        var group = new GroupEntity {Id = groupId};

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
    public async Task CreateLootAsync_WhenGroupDoesNotExists_ShouldThrow()
    {
        var request = new CreateLootRequest {Name = string.Empty};
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(42)
            .Returns((GroupEntity)null);

        Func<Task> act = () => _service.CreateLootAsync(naheulbookExecutionContext, 42, request);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Test]
    public async Task CreateLootAsync_EnsureUserAccessToLoot()
    {
        const int groupId = 42;
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};
        var group = new GroupEntity {Id = groupId};
        var request = new CreateLootRequest {Name = string.Empty};

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);

        _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
            .Throw(new TestException());

        Func<Task> act = () => _service.CreateLootAsync(naheulbookExecutionContext, groupId, request);

        await act.Should().ThrowAsync<TestException>();
    }

    [Test]
    public async Task GetLootsForGroupAsync_ShouldLoadLootsListAndReturnIt()
    {
        const int groupId = 42;
        var executionContext = new NaheulbookExecutionContext();
        var group = new GroupEntity {Id = groupId};
        var expectedLoots = new List<LootEntity>();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);
        _unitOfWorkFactory.GetUnitOfWork().Loots.GetByGroupIdAsync(groupId)
            .Returns(expectedLoots);

        var loots = await _service.GetLootsForGroupAsync(executionContext, groupId);

        loots.Should().BeSameAs(expectedLoots);
    }

    [Test]
    public async Task GetLootsForGroupAsync_ShouldThrowWhenGroupNotFound()
    {
        const int groupId = 42;
        var executionContext = new NaheulbookExecutionContext();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns((GroupEntity)null);

        Func<Task> act = () => _service.GetLootsForGroupAsync(executionContext, groupId);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Test]
    public async Task GetLootsForGroupAsync_ShouldEnsureGroupAccess()
    {
        const int groupId = 42;
        var naheulbookExecutionContext = new NaheulbookExecutionContext();
        var group = new GroupEntity {Id = groupId};

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);

        _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
            .Throw(new TestException());

        Func<Task> act = () => _service.GetLootsForGroupAsync(naheulbookExecutionContext, groupId);

        await act.Should().ThrowAsync<TestException>();
    }
}