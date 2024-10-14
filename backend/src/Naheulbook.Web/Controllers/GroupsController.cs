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

namespace Naheulbook.Web.Controllers;

[Route("api/v2/groups")]
[ApiController]
public class GroupsController(
    IGroupService groupService,
    ILootService lootService,
    IMonsterService monsterService,
    IEventService eventService,
    IFightService fightService,
    IMapper mapper,
    INpcService npcService
) : ControllerBase
{
    [HttpGet]
    public async Task<List<GroupSummaryResponse>> GetGroupListAsync(
        [FromServices] NaheulbookExecutionContext executionContext
    )
    {
        var group = await groupService.GetGroupListAsync(executionContext);
        return mapper.Map<List<GroupSummaryResponse>>(group);
    }

    [HttpPost]
    public async Task<CreatedActionResult<GroupResponse>> PostCreateGroupAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateGroupRequest request
    )
    {
        var group = await groupService.CreateGroupAsync(executionContext, request);
        return mapper.Map<GroupResponse>(group);
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
            await groupService.EditGroupPropertiesAsync(executionContext, groupId, request);
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

    [HttpPatch("{GroupId:int:min(1)}/config")]
    public async Task<IActionResult> PatchGroupConfigAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId,
        PatchGroupConfigRequest request
    )
    {
        try
        {
            await groupService.EditGroupConfigAsync(executionContext, groupId, request);
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

    [HttpPost("{GroupId:int:min(1)}/startCombat")]
    public async Task<IActionResult> PostStartCombatAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId
    )
    {
        try
        {
            await groupService.StartCombatAsync(executionContext, groupId);
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
            await groupService.EndCombatAsync(executionContext, groupId);
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
            var loots = await lootService.GetLootsForGroupAsync(executionContext, groupId);
            return mapper.Map<List<LootResponse>>(loots);
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
            var loot = await lootService.CreateLootAsync(executionContext, groupId, request);
            return mapper.Map<LootResponse>(loot);
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
            var events = await eventService.GetEventsForGroupAsync(executionContext, groupId);
            return mapper.Map<List<EventResponse>>(events);
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
            var groupEvent = await eventService.CreateEventAsync(executionContext, groupId, request);
            return mapper.Map<EventResponse>(groupEvent);
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
            await eventService.DeleteEventAsync(executionContext, groupId, eventId);
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

    [HttpGet("{GroupId:int:min(1)}/fights")]
    public async Task<ActionResult<List<FightResponse>>> GetFightListAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId
    )
    {
        try
        {
            var fights = await fightService.GetFightsForGroupAsync(executionContext, groupId);
            return mapper.Map<List<FightResponse>>(fights);
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

    [HttpPost("{GroupId:int:min(1)}/fights")]
    public async Task<CreatedActionResult<FightResponse>> PostCreateFightAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId,
        CreateFightRequest request
    )
    {
        try
        {
            var groupFight = await fightService.CreateFightAsync(executionContext, groupId, request);
            return mapper.Map<FightResponse>(groupFight);
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


    [HttpPost("{GroupId:int:min(1)}/fights/{FightId:int:min(1)}/start")]
    public async Task<ActionResult<List<FightResponse>>> PostStartFightAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId,
        [FromRoute] int fightId
    )
    {
        try
        {
            await fightService.StartFightAsync(executionContext, groupId, fightId);
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
        catch (FightNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpDelete("{GroupId:int:min(1)}/fights/{FightId:int:min(1)}")]
    public async Task<ActionResult<FightResponse>> PostDeleteFightAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId,
        [FromRoute] int fightId
    )
    {
        try
        {
            await fightService.DeleteFightAsync(executionContext, groupId, fightId);
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
        catch (FightNotFoundException ex)
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
            var monsters = await monsterService.GetMonstersForGroupAsync(executionContext, groupId);
            return mapper.Map<List<MonsterResponse>>(monsters);
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
    public async Task<ActionResult<List<DeadMonsterResponse>>> GetDeadMonsterListAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId,
        [FromQuery] int startIndex,
        [FromQuery] int count
    )
    {
        try
        {
            var monsters = await monsterService.GetDeadMonstersForGroupAsync(executionContext, groupId, startIndex, count);
            return mapper.Map<List<DeadMonsterResponse>>(monsters);
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
            var monster = await monsterService.CreateMonsterAsync(executionContext, groupId, request);
            return mapper.Map<MonsterResponse>(monster);
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
            var invite = await groupService.CreateInviteAsync(executionContext, groupId, request);
            return mapper.Map<GroupInviteResponse>(invite);
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
            var invite = await groupService.CancelOrRejectInviteAsync(executionContext, groupId, characterId);
            return mapper.Map<DeleteInviteResponse>(invite);
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
            await groupService.AcceptInviteAsync(executionContext, groupId, characterId);
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
            var historyEntries = await groupService.GetGroupHistoryEntriesAsync(executionContext, groupId, page);
            return mapper.Map<List<GroupHistoryEntryResponse>>(historyEntries);
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
            await groupService.AddHistoryEntryAsync(executionContext, groupId, request);
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
            var group = await groupService.GetGroupDetailsAsync(executionContext, groupId);
            return mapper.Map<GroupResponse>(group);
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
            await groupService.UpdateDurationsAsync(executionContext, groupId, request);
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
            var characters = await groupService.ListActiveCharactersAsync(executionContext, groupId);
            return mapper.Map<List<ListActiveCharacterResponse>>(characters);
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
            return await groupService.AddTimeAsync(executionContext, groupId, request);
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

    [HttpGet("{GroupId:int:min(1)}/npcs")]
    public async Task<ActionResult<List<NpcResponse>>> GetNpcsAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId
    )
    {
        try
        {
            var npcs = await npcService.LoadNpcsAsync(executionContext, groupId);
            return mapper.Map<List<NpcResponse>>(npcs);
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


    [HttpPost("{GroupId:int:min(1)}/npcs")]
    public async Task<CreatedActionResult<NpcResponse>> PostCreateNpcAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId,
        NpcRequest request
    )
    {
        try
        {
            var npc = await npcService.CreateNpcAsync(executionContext, groupId, request);
            return mapper.Map<NpcResponse>(npc);
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