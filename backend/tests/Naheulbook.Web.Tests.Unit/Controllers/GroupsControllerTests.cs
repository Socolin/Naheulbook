using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Event;
using Naheulbook.Core.Features.Fight;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Loot;
using Naheulbook.Core.Features.Merchant;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Core.Features.Npc;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class GroupsControllerTests
{
    private IGroupService _groupService;
    private ILootService _lootService;
    private IMonsterService _monsterService;
    private IEventService _eventService;
    private IMapper _mapper;
    private INpcService _npcService;
    private NaheulbookExecutionContext _executionContext;
    private IFightService _fightService;
    private IMerchantService _merchantService;

    private GroupsController _controller;

    [SetUp]
    public void SetUp()
    {
        _groupService = Substitute.For<IGroupService>();
        _lootService = Substitute.For<ILootService>();
        _monsterService = Substitute.For<IMonsterService>();
        _eventService = Substitute.For<IEventService>();
        _mapper = Substitute.For<IMapper>();
        _npcService = Substitute.For<INpcService>();
        _fightService = Substitute.For<IFightService>();
        _merchantService = Substitute.For<IMerchantService>();

        _controller = new GroupsController(
            _groupService,
            _lootService,
            _monsterService,
            _eventService,
            _fightService,
            _merchantService,
            _mapper,
            _npcService
        );

        _executionContext = new NaheulbookExecutionContext();
    }

    [Test]
    public async Task PostCreateGroupAsync_ShouldCreateGroupWithGroupService_ThenReturnGroupResponse()
    {
        var createGroupRequest = new CreateGroupRequest {Name = string.Empty};
        var createdGroup = new GroupEntity();
        var groupResponse = new GroupResponse();

        _groupService.CreateGroupAsync(_executionContext, createGroupRequest)
            .Returns(createdGroup);
        _mapper.Map<GroupResponse>(createdGroup)
            .Returns(groupResponse);

        var result = await _controller.PostCreateGroupAsync(_executionContext, createGroupRequest);

        result.Value.Should().BeSameAs(groupResponse);
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Test]
    public async Task PostCreateLootAsync_ShouldCreateLoot_ThenReturnLootResponse()
    {
        const int groupId = 8;
        var createLootRequest = new CreateLootRequest {Name = string.Empty};
        var createdLoot = new LootEntity();
        var lootResponse = new LootResponse();

        _lootService.CreateLootAsync(_executionContext, groupId, createLootRequest)
            .Returns(createdLoot);
        _mapper.Map<LootResponse>(createdLoot)
            .Returns(lootResponse);

        var result = await _controller.PostCreateLootAsync(_executionContext, groupId, createLootRequest);

        result.Value.Should().BeSameAs(lootResponse);
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonGroupExceptionsAndExpectedStatusCode))]
    public async Task PostCreateLootAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        const int groupId = 8;
        var createLootRequest = new CreateLootRequest {Name = string.Empty};

        _lootService.CreateLootAsync(_executionContext, groupId, createLootRequest)
            .Returns(Task.FromException<LootEntity>(exception));

        Func<Task> act = () => _controller.PostCreateLootAsync(_executionContext, groupId, createLootRequest);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    public async Task PostCreateMonsterAsync_ShouldCreateMonster_ThenReturnMonsterResponse()
    {
        const int groupId = 8;
        var createMonsterRequest = new CreateMonsterRequest {Name = string.Empty, Items = new List<CreateItemRequest>()};
        var createdMonster = new MonsterEntity();
        var monsterResponse = new MonsterResponse();

        _monsterService.CreateMonsterAsync(_executionContext, groupId, createMonsterRequest)
            .Returns(createdMonster);
        _mapper.Map<MonsterResponse>(createdMonster)
            .Returns(monsterResponse);

        var result = await _controller.PostCreateMonsterAsync(_executionContext, groupId, createMonsterRequest);

        result.Value.Should().BeSameAs(monsterResponse);
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonGroupExceptionsAndExpectedStatusCode))]
    public async Task PostCreateMonsterAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        const int groupId = 8;
        var createMonsterRequest = new CreateMonsterRequest {Name = string.Empty, Items = new List<CreateItemRequest>()};

        _monsterService.CreateMonsterAsync(_executionContext, groupId, createMonsterRequest)
            .Returns(Task.FromException<MonsterEntity>(exception));

        Func<Task> act = () => _controller.PostCreateMonsterAsync(_executionContext, groupId, createMonsterRequest);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    public async Task PostCreateInviteAsync_ShouldCreateInvite_ThenReturnInviteResponse()
    {
        const int groupId = 8;
        var createInviteRequest = new CreateInviteRequest();
        var createdInvite = new GroupInviteEntity();
        var groupInviteResponse = new GroupInviteResponse();

        _groupService.CreateInviteAsync(_executionContext, groupId, createInviteRequest)
            .Returns(createdInvite);
        _mapper.Map<GroupInviteResponse>(createdInvite)
            .Returns(groupInviteResponse);

        var result = await _controller.PostCreateInviteAsync(_executionContext, groupId, createInviteRequest);

        result.Value.Should().BeSameAs(groupInviteResponse);
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Test]
    [TestCaseSource(nameof(GetPostCreateInviteExceptionsAndExpectedStatusCode))]
    public async Task PostCreateInviteAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        _groupService.CreateInviteAsync(_executionContext, Arg.Any<int>(), Arg.Any<CreateInviteRequest>())
            .Returns(Task.FromException<GroupInviteEntity>(exception));

        Func<Task> act = () => _controller.PostCreateInviteAsync(_executionContext, 8, new CreateInviteRequest());

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    public async Task DeleteInviteAsync_ShouldDeleteInvite_ThenReturnInviteResponse()
    {
        const int characterId = 12;
        const int groupId = 8;
        var groupInvite = new GroupInviteEntity();
        var response = new DeleteInviteResponse();

        _groupService.CancelOrRejectInviteAsync(_executionContext, groupId, characterId)
            .Returns(groupInvite);
        _mapper.Map<DeleteInviteResponse>(groupInvite)
            .Returns(response);

        var result = await _controller.DeleteInviteAsync(_executionContext, groupId, characterId);

        result.Value.Should().BeSameAs(response);
    }

    [Test]
    [TestCaseSource(nameof(GetDeleteInviteExceptionsAndExpectedStatusCode))]
    public async Task DeleteInviteAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        _groupService.CancelOrRejectInviteAsync(_executionContext, Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromException<GroupInviteEntity>(exception));

        Func<Task> act = () => _controller.DeleteInviteAsync(_executionContext, 8, 12);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    [TestCaseSource(nameof(GetAcceptInviteExceptionsAndExpectedStatusCode))]
    public async Task PostAcceptInviteAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        _groupService.AcceptInviteAsync(_executionContext, Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromException<GroupInviteEntity>(exception));

        Func<Task> act = () => _controller.PostAcceptInviteAsync(_executionContext, 8, 12);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    public async Task GetGroupDetailsAsync_ShouldCreateLoot_ThenReturnLootResponse()
    {
        const int groupId = 8;
        var group = new GroupEntity();
        var groupResponse = new GroupResponse();

        _groupService.GetGroupDetailsAsync(_executionContext, groupId)
            .Returns(group);
        _mapper.Map<GroupResponse>(group)
            .Returns(groupResponse);

        var result = await _controller.GetGroupDetailsAsync(_executionContext, groupId);

        result.Value.Should().BeSameAs(groupResponse);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonGroupExceptionsAndExpectedStatusCode))]
    public async Task GetGroupDetailsAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        const int groupId = 8;

        _groupService.GetGroupDetailsAsync(_executionContext, groupId)
            .Returns(Task.FromException<GroupEntity>(exception));

        Func<Task> act = () => _controller.GetGroupDetailsAsync(_executionContext, groupId);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    public async Task GetEventListAsync_LoadEventThenMapTheResponse()
    {
        const int groupId = 8;
        var events = new List<EventEntity>();
        var eventsResponse = new List<EventResponse>();

        _eventService.GetEventsForGroupAsync(_executionContext, groupId)
            .Returns(events);
        _mapper.Map<List<EventResponse>>(events)
            .Returns(eventsResponse);

        var result = await _controller.GetEventListAsync(_executionContext, groupId);

        result.Value.Should().BeSameAs(eventsResponse);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonGroupExceptionsAndExpectedStatusCode))]
    public async Task GetEventListAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        const int groupId = 8;

        _eventService.GetEventsForGroupAsync(_executionContext, groupId)
            .Returns(Task.FromException<List<EventEntity>>(exception));

        Func<Task> act = () => _controller.GetEventListAsync(_executionContext, groupId);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    public async Task GetEventListAsync_LoadLootThenMapTheResponse()
    {
        const int groupId = 8;
        var loots = new List<LootEntity>();
        var lootsResponse = new List<LootResponse>();

        _lootService.GetLootsForGroupAsync(_executionContext, groupId)
            .Returns(loots);
        _mapper.Map<List<LootResponse>>(loots)
            .Returns(lootsResponse);

        var result = await _controller.GetLootListAsync(_executionContext, groupId);

        result.Value.Should().BeSameAs(lootsResponse);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonGroupExceptionsAndExpectedStatusCode))]
    public async Task GetLootListAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        const int groupId = 8;

        _lootService.GetLootsForGroupAsync(_executionContext, groupId)
            .Returns(Task.FromException<List<LootEntity>>(exception));

        Func<Task> act = () => _controller.GetLootListAsync(_executionContext, groupId);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonGroupExceptionsAndExpectedStatusCode))]
    public async Task PatchGroupAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        const int groupId = 8;
        var request = new PatchGroupRequest();

        _groupService.EditGroupPropertiesAsync(_executionContext, groupId, request)
            .Returns(Task.FromException<List<LootEntity>>(exception));

        Func<Task> act = () => _controller.PatchGroupAsync(_executionContext, groupId, request);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    #region GetMonstersForGroupAsync

    [Test]
    public async Task GetMonsterListAsync_LoadMonsterThenMapTheResponse()
    {
        const int groupId = 8;
        var monsters = new List<MonsterEntity>();
        var monstersResponse = new List<MonsterResponse>();

        _monsterService.GetMonstersForGroupAsync(_executionContext, groupId)
            .Returns(monsters);
        _mapper.Map<List<MonsterResponse>>(monsters)
            .Returns(monstersResponse);

        var result = await _controller.GetMonsterListAsync(_executionContext, groupId);

        result.Value.Should().BeSameAs(monstersResponse);
    }

    [Test]
    [TestCaseSource(nameof(GetCommonGroupExceptionsAndExpectedStatusCode))]
    public async Task GetMonsterListAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
    {
        const int groupId = 8;

        _monsterService.GetMonstersForGroupAsync(_executionContext, groupId)
            .Returns(Task.FromException<List<MonsterEntity>>(exception));

        Func<Task> act = () => _controller.GetMonsterListAsync(_executionContext, groupId);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
    }

    #endregion

    #region CreateMerchantAsync

    [Test]
    public async Task CreateMerchantAsync_ShouldCreateNewMerchant_AndReturnCreatedData()
    {
        const int groupId = 1;
        var request = new CreateMerchantRequest
        {
            Name = "some-name",
        };
        var expectedResponse = new MerchantResponse {Id = 42, Name = "some-name"};
        var merchant = new MerchantEntity();

        _merchantService.CreateAsync(_executionContext, groupId, request)
            .Returns(merchant);
        _mapper.Map<MerchantResponse>(merchant)
            .Returns(expectedResponse);

        var actual = await _controller.CreateMerchantAsync(_executionContext, groupId, request);

        actual.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Test]
    public async Task CreateMerchantAsync_WhenTheGroupDoesNotExists_ReturnsNotFound()
    {
        const int groupId = 1;
        var request = new CreateMerchantRequest {Name = "some-name"};

        _merchantService.CreateAsync(_executionContext, groupId, request)
            .ThrowsAsync(new GroupNotFoundException(groupId));

        var act = () => _controller.CreateMerchantAsync(_executionContext, groupId, request);

        (await act.Should().ThrowAsync<HttpErrorException>())
            .Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

    private static IEnumerable<TestCaseData> GetCommonGroupExceptionsAndExpectedStatusCode()
    {
        yield return new TestCaseData(new ForbiddenAccessException(), StatusCodes.Status403Forbidden);
        yield return new TestCaseData(new GroupNotFoundException(42), StatusCodes.Status404NotFound);
    }

    private static IEnumerable<TestCaseData> GetPostCreateInviteExceptionsAndExpectedStatusCode()
    {
        foreach (var testCaseData in GetCommonGroupExceptionsAndExpectedStatusCode())
            yield return testCaseData;

        yield return new TestCaseData(new CharacterNotFoundException(42), StatusCodes.Status400BadRequest);
        yield return new TestCaseData(new CharacterAlreadyInAGroupException(42), StatusCodes.Status400BadRequest);
    }

    private static IEnumerable<TestCaseData> GetDeleteInviteExceptionsAndExpectedStatusCode()
    {
        yield return new TestCaseData(new ForbiddenAccessException(), StatusCodes.Status403Forbidden);
        yield return new TestCaseData(new InviteNotFoundException(42, 8), StatusCodes.Status404NotFound);
    }

    private static IEnumerable<TestCaseData> GetAcceptInviteExceptionsAndExpectedStatusCode()
    {
        yield return new TestCaseData(new ForbiddenAccessException(), StatusCodes.Status403Forbidden);
        yield return new TestCaseData(new InviteNotFoundException(42, 8), StatusCodes.Status404NotFound);
        yield return new TestCaseData(new CharacterAlreadyInAGroupException(42), StatusCodes.Status400BadRequest);
    }
}