using System;
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
        private IMapper _mapper;
        private NaheulbookExecutionContext _executionContext;

        private GroupsController _controller;

        [SetUp]
        public void SetUp()
        {
            _groupService = Substitute.For<IGroupService>();
            _lootService = Substitute.For<ILootService>();
            _mapper = Substitute.For<IMapper>();

            _controller = new GroupsController(_groupService, _lootService, _mapper);

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
        public void PostCreateLootAsync_ShouldReturn403_WhenUserDoesNotHaveAccess()
        {
            const int groupId = 8;
            var createLootRequest = new CreateLootRequest();

            _lootService.CreateLootAsync(_executionContext, groupId, createLootRequest)
                .Returns(Task.FromException<Loot>(new ForbiddenAccessException()));

            Func<Task> act = () =>  _controller.PostCreateLootAsync(_executionContext, groupId, createLootRequest);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public void PostCreateLootAsync_ShouldReturn404_WhenGroupDoesNotExists()
        {
            const int groupId = 8;
            var createLootRequest = new CreateLootRequest();

            _lootService.CreateLootAsync(_executionContext, groupId, createLootRequest)
                .Returns(Task.FromException<Loot>(new GroupNotFoundException(groupId)));

            Func<Task> act = () =>  _controller.PostCreateLootAsync(_executionContext, groupId, createLootRequest);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}