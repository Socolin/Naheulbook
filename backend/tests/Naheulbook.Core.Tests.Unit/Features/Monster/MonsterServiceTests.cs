using FluentAssertions;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Monster;

public class MonsterServiceTests
{
    private FakeUnitOfWorkFactory _unitOfWorkFactory;
    private IAuthorizationUtil _authorizationUtil;
    private IActiveStatsModifierUtil _activeStatsModifierUtil;
    private FakeNotificationSessionFactory _notificationSessionFactory;
    private IJsonUtil _jsonUtil;
    private ITimeService _timeService;
    private MonsterService _service;
    private IItemService _itemService;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkFactory = new FakeUnitOfWorkFactory();
        _authorizationUtil = Substitute.For<IAuthorizationUtil>();
        _activeStatsModifierUtil = Substitute.For<IActiveStatsModifierUtil>();
        _notificationSessionFactory = new FakeNotificationSessionFactory();
        _jsonUtil = Substitute.For<IJsonUtil>();
        _timeService = Substitute.For<ITimeService>();
        _itemService = Substitute.For<IItemService>();

        _service = new MonsterService(
            _unitOfWorkFactory,
            _authorizationUtil,
            _activeStatsModifierUtil,
            _notificationSessionFactory,
            _jsonUtil,
            _timeService,
            _itemService
        );
    }

    [Test]
    public async Task CreateMonsterAsync_ShouldCreateNewEntryInDatabase_AndLinkItToGroup()
    {
        const int groupId = 42;
        var request = CreateRequest();
        var executionContext = new NaheulbookExecutionContext();
        var group = new GroupEntity {Id = groupId};

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);
        _jsonUtil.Serialize(request.Data)
            .Returns("some-json-data");
        _jsonUtil.Serialize(request.Modifiers)
            .Returns("some-json-modifiers");

        var actualMonster = await _service.CreateMonsterAsync(executionContext, groupId, request);

        Received.InOrder(() =>
        {
            _activeStatsModifierUtil.InitializeModifierIds(request.Modifiers);
            _unitOfWorkFactory.GetUnitOfWork().Monsters.Add(actualMonster);
            _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });

        actualMonster.Should().BeEquivalentTo(new MonsterEntity
        {
            Name = "some-monster-name",
            Data = "some-json-data",
            Modifiers = "some-json-modifiers",
            Group = group,
        });
    }

    [Test]
    public async Task CreateMonsterAsync_ShouldCreateInventoryItems_ThenReturnsMonsterWithFullyLoadedItems()
    {
        const int groupId = 42;
        const int itemId = 24;
        var request = CreateRequest();
        request.Items = new List<CreateItemRequest> {new()};
        var executionContext = new NaheulbookExecutionContext();
        var group = new GroupEntity {Id = groupId};
        var items = new List<ItemEntity> {new() {Id = itemId}};
        var fullyLoadedItems = new List<ItemEntity>();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);
        _jsonUtil.Serialize(request.Data)
            .Returns("some-json-data");
        _jsonUtil.Serialize(request.Modifiers)
            .Returns("some-json-modifiers");
        _itemService.CreateItemsAsync(request.Items)
            .Returns(items);
        _unitOfWorkFactory.GetUnitOfWork().Items.GetWithAllDataByIdsAsync(Arg.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] {itemId})))
            .Returns(fullyLoadedItems);

        var actualMonster = await _service.CreateMonsterAsync(executionContext, groupId, request);

        actualMonster.Items.Should().BeSameAs(fullyLoadedItems);

    }

    [Test]
    public async Task CreateMonsterAsync_ShouldThrowWhenGroupNotFound()
    {
        const int groupId = 42;
        var request = CreateRequest();
        var executionContext = new NaheulbookExecutionContext();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns((GroupEntity) null);

        Func<Task> act = () => _service.CreateMonsterAsync(executionContext, groupId, request);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Test]
    public async Task CreateMonsterAsync_EnsureUserIsOwnerOfGroup()
    {
        const int groupId = 42;
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};
        var group = new GroupEntity {Id = groupId};
        var request = new CreateMonsterRequest {Name = string.Empty, Items = new List<CreateItemRequest>()};

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);

        _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
            .Throw(new TestException());

        Func<Task> act = () => _service.CreateMonsterAsync(naheulbookExecutionContext, groupId, request);

        await act.Should().ThrowAsync<TestException>();
    }


    [Test]
    public async Task GetMonstersForGroupAsync_ShouldLoadMonstersListAndReturnIt()
    {
        const int groupId = 42;
        var executionContext = new NaheulbookExecutionContext();
        var group = new GroupEntity {Id = groupId};
        var expectedMonsters = new List<MonsterEntity>();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);
        _unitOfWorkFactory.GetUnitOfWork().Monsters.GetByGroupIdWithInventoryAsync(groupId)
            .Returns(expectedMonsters);

        var events = await _service.GetMonstersForGroupAsync(executionContext, groupId);

        events.Should().BeSameAs(expectedMonsters);
    }

    [Test]
    public async Task GetMonstersForGroupAsync_ShouldThrowWhenGroupNotFound()
    {
        const int groupId = 42;
        var executionContext = new NaheulbookExecutionContext();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns((GroupEntity) null);

        Func<Task> act = () => _service.GetMonstersForGroupAsync(executionContext, groupId);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Test]
    public async Task GetMonstersForGroupAsync_ShouldEnsureGroupAccess()
    {
        const int groupId = 42;
        var naheulbookExecutionContext = new NaheulbookExecutionContext();
        var group = new GroupEntity {Id = groupId};

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);

        _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
            .Throw(new TestException());

        Func<Task> act = () => _service.GetMonstersForGroupAsync(naheulbookExecutionContext, groupId);

        await act.Should().ThrowAsync<TestException>();
    }

    private static CreateMonsterRequest CreateRequest()
    {
        return new CreateMonsterRequest
        {
            Name = "some-monster-name",
            Data = new MonsterData {At = 8, Prd = 10},
            Items = new List<CreateItemRequest>(),
            Modifiers = new List<ActiveStatsModifier>(),
        };
    }
}