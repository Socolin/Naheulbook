using System.Collections.Generic;
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
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;

        public GroupsController(
            IGroupService groupService,
            ILootService lootService,
            IMonsterService monsterService,
            IEventService eventService,
            IMapper mapper
        )
        {
            _groupService = groupService;
            _lootService = lootService;
            _monsterService = monsterService;
            _eventService = eventService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<List<GroupSummaryResponse>> GetGroupList(
            [FromServices] NaheulbookExecutionContext executionContext
        )
        {
            var group = await _groupService.GetGroupListAsync(executionContext);
            return _mapper.Map<List<GroupSummaryResponse>>(group);
        }

        [HttpPost]
        public async Task<CreatedActionResult<GroupResponse>> PostCreateGroupAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateGroupRequest request
        )
        {
            var group = await _groupService.CreateGroupAsync(executionContext, request);
            return _mapper.Map<GroupResponse>(group);
        }

        [HttpGet("{GroupId:int:min(1)}/loots")]
        public async Task<ActionResult<List<LootResponse>>> GetLootListAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId
        )
        {
            try
            {
                var loots = await _lootService.GetLootsForGroupAsync(executionContext, groupId);
                return _mapper.Map<List<LootResponse>>(loots);
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

        [HttpPost("{GroupId:int:min(1)}/loots")]
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

        [HttpGet("{GroupId:int:min(1)}/events")]
        public async Task<ActionResult<List<EventResponse>>> GetEventListAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId
        )
        {
            try
            {
                var events = await _eventService.GetEventsForGroupAsync(executionContext, groupId);
                return _mapper.Map<List<EventResponse>>(events);
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

        [HttpGet("{GroupId:int:min(1)}/monsters")]
        public async Task<ActionResult<List<MonsterResponse>>> GetMonsterListAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId
        )
        {
            try
            {
                var monsters = await _monsterService.GetMonstersForGroupAsync(executionContext, groupId);
                return _mapper.Map<List<MonsterResponse>>(monsters);
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

        [HttpGet("{GroupId:int:min(1)}/deadMonsters")]
        public async Task<ActionResult<List<MonsterResponse>>> GetDeadMonsterListAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            [FromQuery] int startIndex,
            [FromQuery] int count
        )
        {
            try
            {
                var monsters = await _monsterService.GetDeadMonstersForGroupAsync(executionContext, groupId, startIndex, count);
                return _mapper.Map<List<MonsterResponse>>(monsters);
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

        [HttpPost("{GroupId:int:min(1)}/monsters")]
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

        [HttpPost("{GroupId:int:min(1)}/invites")]
        public async Task<CreatedActionResult<GroupInviteResponse>> PostCreateInviteAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            CreateInviteRequest request
        )
        {
            try
            {
                var invite = await _groupService.CreateInviteAsync(executionContext, groupId, request);
                return _mapper.Map<GroupInviteResponse>(invite);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
            catch (CharacterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.BadRequest, ex);
            }
            catch (CharacterAlreadyInAGroupException ex)
            {
                throw new HttpErrorException(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete("{GroupId:int:min(1)}/invites/{CharacterId:int:min(1)}")]
        public async Task<ActionResult<DeleteInviteResponse>> DeleteInviteAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            [FromRoute] int characterId
        )
        {
            try
            {
                var invite = await _groupService.CancelOrRejectInviteAsync(executionContext, groupId, characterId);
                return _mapper.Map<DeleteInviteResponse>(invite);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (InviteNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPost("{GroupId:int:min(1)}/invites/{CharacterId:int:min(1)}/accept")]
        public async Task<ActionResult> PostAcceptInviteAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            [FromRoute] int characterId
        )
        {
            try
            {
                await _groupService.AcceptInviteAsync(executionContext, groupId, characterId);
                return new NoContentResult();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (InviteNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
            catch (CharacterAlreadyInAGroupException ex)
            {
                throw new HttpErrorException(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet("{GroupId:int:min(1)}/history")]
        public async Task<ActionResult<List<GroupHistoryEntryResponse>>> PostCreateMonsterAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            [FromQuery] int page
        )
        {
            try
            {
                var monster = await _groupService.GetGroupHistoryEntriesAsync(executionContext, groupId, page);
                return _mapper.Map<List<GroupHistoryEntryResponse>>(monster);
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

        [HttpGet("{GroupId:int:min(1)}")]
        public async Task<ActionResult<GroupResponse>> GetGroupDetailsAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId
        )
        {
            try
            {
                var group = await _groupService.GetGroupDetailsAsync(executionContext, groupId);
                return _mapper.Map<GroupResponse>(group);
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