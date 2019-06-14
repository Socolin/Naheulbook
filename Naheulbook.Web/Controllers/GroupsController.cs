using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Filters;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/groups")]
    [ApiController]
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly ILootService _lootService;
        private readonly IMapper _mapper;

        public GroupsController(IGroupService groupService, ILootService lootService, IMapper mapper)
        {
            _groupService = groupService;
            _lootService = lootService;
            _mapper = mapper;
        }

        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        [HttpPost]
        public async Task<CreatedActionResult<GroupResponse>> PostCreateGroupAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateGroupRequest request
        )
        {
            var group = await _groupService.CreateGroupAsync(executionContext, request);
            return _mapper.Map<GroupResponse>(group);
        }

        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        [HttpPost("{GroupId}/loots")]
        public async Task<CreatedActionResult<LootResponse>> PostCreateLootAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            CreateLootRequest request
        )
        {
            try
            {
                var loot = await _lootService.CreateLootAsync(executionContext, groupId, request);
                return _mapper.Map<LootResponse>(loot);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }
    }
}