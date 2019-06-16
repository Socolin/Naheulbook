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
        private readonly IMonsterService _monsterService;
        private readonly IMapper _mapper;

        public GroupsController(
            IGroupService groupService,
            ILootService lootService,
            IMonsterService monsterService,
            IMapper mapper
        )
        {
            _groupService = groupService;
            _lootService = lootService;
            _monsterService = monsterService;
            _mapper = mapper;
        }

        [HttpPost]
        [ServiceFilter(typeof(JwtAuthorizationFilter))]
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


        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        [HttpPost("{GroupId}/monsters")]
        public async Task<CreatedActionResult<MonsterResponse>> PostCreateMonsterAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            CreateMonsterRequest request
        )
        {
            try
            {
                var monster = await _monsterService.CreateMonsterAsync(executionContext, groupId, request);
                return _mapper.Map<MonsterResponse>(monster);
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