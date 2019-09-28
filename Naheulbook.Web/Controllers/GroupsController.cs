using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        public async Task<List<GroupSummaryResponse>> GetGroupListAsync(
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

        [HttpPatch("{GroupId:int:min(1)}")]
        public async Task<IActionResult> PatchGroupAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            PatchGroupRequest request
        )
        {
            try
            {
                await _groupService.EditGroupPropertiesAsync(executionContext, groupId, request);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }

        [HttpPut("{GroupId:int:min(1)}/location")]
        public async Task<IActionResult> PutChangeGroupLocationAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            PutChangeLocationRequest request
        )
        {
            try
            {
                await _groupService.EditGroupLocationAsync(executionContext, groupId, request);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
            catch (LocationNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
            }
        }

        [HttpPost("{GroupId:int:min(1)}/startCombat")]
        public async Task<IActionResult> PostStartCombatAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId
        )
        {
            try
            {
                await _groupService.StartCombatAsync(executionContext, groupId);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
            catch (GroupAlreadyInCombatException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
            }
        }

        [HttpPost("{GroupId:int:min(1)}/endCombat")]
        public async Task<IActionResult> PostEndCombatAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId
        )
        {
            try
            {
                await _groupService.EndCombatAsync(executionContext, groupId);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
            catch (GroupNotInCombatException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
            }
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
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
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
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
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
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }

        [HttpPost("{GroupId:int:min(1)}/events")]
        public async Task<CreatedActionResult<EventResponse>> PostCreateEventAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            CreateEventRequest request
        )
        {
            try
            {
                var groupEvent = await _eventService.CreateEventAsync(executionContext, groupId, request);
                return _mapper.Map<EventResponse>(groupEvent);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }

        [HttpDelete("{GroupId:int:min(1)}/events/{EventId:int:min(1)}")]
        public async Task<ActionResult<EventResponse>> PostDeleteEventAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            [FromRoute] int eventId
        )
        {
            try
            {
                await _eventService.DeleteEventAsync(executionContext, groupId, eventId);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
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
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
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
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
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
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
            catch (ItemTemplateNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
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
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
            catch (CharacterNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
            }
            catch (CharacterAlreadyInAGroupException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
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
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (InviteNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
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
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (InviteNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
            catch (CharacterAlreadyInAGroupException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
            }
        }

        [HttpGet("{GroupId:int:min(1)}/history")]
        public async Task<ActionResult<List<GroupHistoryEntryResponse>>> GetGroupHistoryAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            [FromQuery] int page
        )
        {
            try
            {
                var historyEntries = await _groupService.GetGroupHistoryEntriesAsync(executionContext, groupId, page);
                return _mapper.Map<List<GroupHistoryEntryResponse>>(historyEntries);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }

        [HttpPost("{GroupId:int:min(1)}/history")]
        public async Task<ActionResult<List<GroupHistoryEntryResponse>>> PostCreateGroupHistoryLogAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            PostCreateGroupHistoryEntryRequest request
        )
        {
            try
            {
                await _groupService.AddHistoryEntryAsync(executionContext, groupId, request);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
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
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }

        [HttpPost("{GroupId:int:min(1)}/updateDurations")]
        public async Task<ActionResult<List<MonsterResponse>>> PostUpdateDurationsAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            IList<PostGroupUpdateDurationsRequest> request
        )
        {
            try
            {
                await _groupService.UpdateDurationsAsync(executionContext, groupId, request);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }

        [HttpGet("{GroupId:int:min(1)}/activeCharacters")]
        public async Task<ActionResult<List<ListActiveCharacterResponse>>> GetActiveCharactersAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId
        )
        {
            try
            {
                var characters = await _groupService.ListActiveCharactersAsync(executionContext, groupId);
                return _mapper.Map<List<ListActiveCharacterResponse>>(characters);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }

        [HttpPost("{GroupId:int:min(1)}/addTime")]
        public async Task<ActionResult<NhbkDate>> PostAddTimeAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int groupId,
            NhbkDateOffset request
        )
        {
            try
            {
                return await _groupService.AddTimeAsync(executionContext, groupId, request);
            }
            catch (GroupDateNotSetException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (GroupNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }
    }
}