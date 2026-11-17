using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Event;
using Naheulbook.Core.Features.Fight;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Loot;
using Naheulbook.Core.Features.Merchant;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Core.Features.Npc;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/groups")]
[ApiController]
public class GroupsController(
    IGroupService groupService,
    ILootService lootService,
    IMonsterService monsterService,
    IEventService eventService,
    IFightService fightService,
    IMerchantService merchantService,
    ICharacterMapper characterMapper,
    IEventMapper eventMapper,
    IFightMapper fightMapper,
    IGroupMapper groupMapper,
    ILootMapper lootMapper,
    IMerchantMapper merchantMapper,
    IMonsterMapper monsterMapper,
    INpcMapper npcMapper,
    INpcService npcService
) : ControllerBase
{
    [HttpGet]
    public async Task<List<GroupSummaryResponse>> GetGroupListAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CancellationToken cancellationToken
    )
    {
        var group = await groupService.GetGroupListAsync(executionContext, cancellationToken);
        return group.Select(groupMapper.ToSummaryResponse).ToList();
    }

    [HttpPost]
    public async Task<CreatedActionResult<GroupResponse>> PostCreateGroupAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateGroupRequest request,
        CancellationToken cancellationToken
    )
    {
        var group = await groupService.CreateGroupAsync(executionContext, request, cancellationToken);
        return groupMapper.ToResponse(group);
    }

    [HttpPatch("{GroupId:int:min(1)}")]
    public async Task<IActionResult> PatchGroupAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId,
        PatchGroupRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await groupService.EditGroupPropertiesAsync(executionContext, groupId, request, cancellationToken);
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
        PatchGroupConfigRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await groupService.EditGroupConfigAsync(executionContext, groupId, request, cancellationToken);
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
        [FromRoute] int groupId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await groupService.StartCombatAsync(executionContext, groupId, cancellationToken);
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
        [FromRoute] int groupId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await groupService.EndCombatAsync(executionContext, groupId, cancellationToken);
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
        [FromRoute] int groupId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var loots = await lootService.GetLootsForGroupAsync(executionContext, groupId, cancellationToken);
            return loots.Select(lootMapper.ToResponse).ToList();
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
        CreateLootRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var loot = await lootService.CreateLootAsync(executionContext, groupId, request, cancellationToken);
            return lootMapper.ToResponse(loot);
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
        [FromRoute] int groupId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var events = await eventService.GetEventsForGroupAsync(executionContext, groupId, cancellationToken);
            return events.Select(eventMapper.ToResponse).ToList();
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
        CreateEventRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var groupEvent = await eventService.CreateEventAsync(executionContext, groupId, request, cancellationToken);
            return eventMapper.ToResponse(groupEvent);
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
        [FromRoute] int eventId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await eventService.DeleteEventAsync(executionContext, groupId, eventId, cancellationToken);
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

    [HttpPost("{GroupId:int:min(1)}/merchants")]
    public async Task<CreatedActionResult<MerchantResponse>> CreateMerchantAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId,
        [FromBody] CreateMerchantRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var merchant = await merchantService.CreateAsync(executionContext, groupId, request, cancellationToken);
            return merchantMapper.ToResponse(merchant);
        }
        catch (GroupNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpGet("{GroupId:int:min(1)}/fights")]
    public async Task<ActionResult<List<FightResponse>>> GetFightListAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var fights = await fightService.GetFightsForGroupAsync(executionContext, groupId, cancellationToken);
            return fights.Select(fightMapper.ToResponse).ToList();
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
        CreateFightRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var groupFight = await fightService.CreateFightAsync(executionContext, groupId, request, cancellationToken);
            return fightMapper.ToResponse(groupFight);
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
        [FromRoute] int fightId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await fightService.StartFightAsync(executionContext, groupId, fightId, cancellationToken);
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
        [FromRoute] int fightId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await fightService.DeleteFightAsync(executionContext, groupId, fightId, cancellationToken);
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
        [FromRoute] int groupId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var monsters = await monsterService.GetMonstersForGroupAsync(executionContext, groupId, cancellationToken);
            return monsters.Select(monsterMapper.ToResponse).ToList();
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
        [FromQuery] int count,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var monsters = await monsterService.GetDeadMonstersForGroupAsync(executionContext, groupId, startIndex, count, cancellationToken);
            return monsters.Select(monsterMapper.ToDeadResponse).ToList();
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
        CreateMonsterRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var monster = await monsterService.CreateMonsterAsync(executionContext, groupId, request, cancellationToken);
            return monsterMapper.ToResponse(monster);
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
        CreateInviteRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var invite = await groupService.CreateInviteAsync(executionContext, groupId, request, cancellationToken);
            return groupMapper.ToInviteResponse(invite);
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
        [FromRoute] int characterId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var invite = await groupService.CancelOrRejectInviteAsync(executionContext, groupId, characterId, cancellationToken);
            return groupMapper.ToDeleteInviteResponse(invite);
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
        [FromRoute] int characterId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await groupService.AcceptInviteAsync(executionContext, groupId, characterId, cancellationToken);
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
        [FromQuery] int page,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var historyEntries = await groupService.GetGroupHistoryEntriesAsync(executionContext, groupId, page, cancellationToken);
            return historyEntries.Select(groupMapper.ToResponse).ToList();
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
        PostCreateGroupHistoryEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await groupService.AddHistoryEntryAsync(executionContext, groupId, request, cancellationToken);
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
        [FromRoute] int groupId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var group = await groupService.GetGroupDetailsAsync(executionContext, groupId, cancellationToken);
            return groupMapper.ToResponse(group);
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
        IList<PostGroupUpdateDurationsRequest> request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await groupService.UpdateDurationsAsync(executionContext, groupId, request, cancellationToken);
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
        [FromRoute] int groupId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var characters = await groupService.ListActiveCharactersAsync(executionContext, groupId, cancellationToken);
            return characters.Select(characterMapper.ToListActiveResponse).ToList();
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
        NhbkDateOffset request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await groupService.AddTimeAsync(executionContext, groupId, request, cancellationToken);
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
        [FromRoute] int groupId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var npcs = await npcService.LoadNpcsAsync(executionContext, groupId, cancellationToken);
            return npcs.Select(npcMapper.ToResponse).ToList();
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
        NpcRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var npc = await npcService.CreateNpcAsync(executionContext, groupId, request, cancellationToken);
            return npcMapper.ToResponse(npc);
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