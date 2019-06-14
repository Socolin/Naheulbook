using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class GroupsControllerTests
    {
        private IGroupService _groupService;
        private IMapper _mapper;
        private NaheulbookExecutionContext _executionContext;

        private GroupsController _controller;

        [SetUp]
        public void SetUp()
        {
            _groupService = Substitute.For<IGroupService>();
            _mapper = Substitute.For<IMapper>();

            _controller = new GroupsController(_groupService, _mapper);

            _executionContext = new NaheulbookExecutionContext();
        }

        [Test]
        public async Task PostCreateGroup_ShouldCreateGroupWithGroupService_ThenReturnGroupResponse()
        {
            var createGroupRequest = new CreateGroupRequest();
            var createdGroup = new Group();
            var groupResponse = new GroupResponse();

            _groupService.CreateGroupAsync(_executionContext, createGroupRequest)
                .Returns(createdGroup);
            _mapper.Map<GroupResponse>(createdGroup)
                .Returns(groupResponse);

            var result = await _controller.PostCreateTypeAsync(_executionContext, createGroupRequest);

            result.Value.Should().BeSameAs(groupResponse);
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}