using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class GroupsControllerTests
    {
        private IGroupService _groupService;
        private ILootService _lootService;
        private IMonsterService _monsterService;
        private IEventService _eventService;
        private IMapper _mapper;
        private NaheulbookExecutionContext _executionContext;

        private GroupsController _controller;

        [SetUp]
        public void SetUp()
        {
            _groupService = Substitute.For<IGroupService>();
            _lootService = Substitute.For<ILootService>();
            _monsterService = Substitute.For<IMonsterService>();
            _eventService = Substitute.For<IEventService>();
            _mapper = Substitute.For<IMapper>();

            _controller = new GroupsController(
                _groupService,
                _lootService,
                _monsterService,
                _eventService,
                _mapper
            );

            _executionContext = new NaheulbookExecutionContext();
        }

        [Test]
        public async Task PostCreateGroupAsync_ShouldCreateGroupWithGroupService_ThenReturnGroupResponse()
        {
            var createGroupRequest = new CreateGroupRequest();
            var createdGroup = new Group();
            var groupResponse = new GroupResponse();

            _groupService.CreateGroupAsync(_executionContext, createGroupRequest)
                .Returns(createdGroup);
            _mapper.Map<GroupResponse>(createdGroup)
                .Returns(groupResponse);

            var result = await _controller.PostCreateGroupAsync(_executionContext, createGroupRequest);

            result.Value.Should().BeSameAs(groupResponse);
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task PostCreateLootAsync_ShouldCreateLoot_ThenReturnLootResponse()
        {
            const int groupId = 8;
            var createLootRequest = new CreateLootRequest();
            var createdLoot = new Loot();
            var lootResponse = new LootResponse();

            _lootService.CreateLootAsync(_executionContext, groupId, createLootRequest)
                .Returns(createdLoot);
            _mapper.Map<LootResponse>(createdLoot)
                .Returns(lootResponse);

            var result = await _controller.PostCreateLootAsync(_executionContext, groupId, createLootRequest);

            result.Value.Should().BeSameAs(lootResponse);
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonGroupExceptionsAndExpectedStatusCode))]
        public void PostCreateLootAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            const int groupId = 8;
            var createLootRequest = new CreateLootRequest();

            _lootService.CreateLootAsync(_executionContext, groupId, createLootRequest)
                .Returns(Task.FromException<Loot>(exception));

            Func<Task> act = () => _controller.PostCreateLootAsync(_executionContext, groupId, createLootRequest);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        public async Task PostCreateMonsterAsync_ShouldCreateMonster_ThenReturnMonsterResponse()
        {
            const int groupId = 8;
            var createMonsterRequest = new CreateMonsterRequest();
            var createdMonster = new Monster();
            var monsterResponse = new MonsterResponse();

            _monsterService.CreateMonsterAsync(_executionContext, groupId, createMonsterRequest)
                .Returns(createdMonster);
            _mapper.Map<MonsterResponse>(createdMonster)
                .Returns(monsterResponse);

            var result = await _controller.PostCreateMonsterAsync(_executionContext, groupId, createMonsterRequest);

            result.Value.Should().BeSameAs(monsterResponse);
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonGroupExceptionsAndExpectedStatusCode))]
        public void PostCreateMonsterAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            const int groupId = 8;
            var createMonsterRequest = new CreateMonsterRequest();

            _monsterService.CreateMonsterAsync(_executionContext, groupId, createMonsterRequest)
                .Returns(Task.FromException<Monster>(exception));

            Func<Task> act = () => _controller.PostCreateMonsterAsync(_executionContext, groupId, createMonsterRequest);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        public async Task GetGroupDetailsAsync_ShouldCreateLoot_ThenReturnLootResponse()
        {
            const int groupId = 8;
            var group = new Group();
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
        public void GetGroupDetailsAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            const int groupId = 8;

            _groupService.GetGroupDetailsAsync(_executionContext, groupId)
                .Returns(Task.FromException<Group>(exception));

            Func<Task> act = () => _controller.GetGroupDetailsAsync(_executionContext, groupId);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        public async Task GetEventListAsync_LoadEventThenMapTheResponse()
        {
            const int groupId = 8;
            var events = new List<Event>();
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
        public void GetEventListAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            const int groupId = 8;

            _eventService.GetEventsForGroupAsync(_executionContext, groupId)
                .Returns(Task.FromException<List<Event>>(exception));

            Func<Task> act = () => _controller.GetEventListAsync(_executionContext, groupId);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        public async Task GetMonsterListAsync_LoadMonsterThenMapTheResponse()
        {
            const int groupId = 8;
            var monsters = new List<Monster>();
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
        public void GetMonsterListAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, HttpStatusCode expectedStatusCode)
        {
            const int groupId = 8;

            _monsterService.GetMonstersForGroupAsync(_executionContext, groupId)
                .Returns(Task.FromException<List<Monster>>(exception));

            Func<Task> act = () => _controller.GetMonsterListAsync(_executionContext, groupId);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        private static IEnumerable<TestCaseData> GetCommonGroupExceptionsAndExpectedStatusCode()
        {
            yield return new TestCaseData(new ForbiddenAccessException(), HttpStatusCode.Forbidden);
            yield return new TestCaseData(new GroupNotFoundException(42), HttpStatusCode.NotFound);
        }

    }
}