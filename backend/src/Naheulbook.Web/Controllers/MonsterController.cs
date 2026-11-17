using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[ApiController]
[Route("api/v2/monsters")]
public class MonsterController(
    IMonsterService monsterService,
    IItemMapper itemMapper,
    IMonsterMapper monsterMapper
) : ControllerBase
{
    [HttpGet("{MonsterId}")]
    public async Task<ActionResult<MonsterResponse>> GetMonsterAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var monster = await monsterService.GetMonsterAsync(executionContext, monsterId, cancellationToken);
            return monsterMapper.ToResponse(monster);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpDelete("{MonsterId}")]
    public async Task<CreatedActionResult<ActiveStatsModifier>> DeleteMonsterAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await monsterService.DeleteMonsterAsync(executionContext, monsterId, cancellationToken);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{MonsterId}/kill")]
    public async Task<CreatedActionResult<ActiveStatsModifier>> PostKillMonsterAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await monsterService.KillMonsterAsync(executionContext, monsterId, cancellationToken);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{MonsterId}/moveToFight")]
    public async Task<CreatedActionResult<ActiveStatsModifier>> PostMoveMonsterToFightAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterId,
        [FromBody] MoveMonsterToFightRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await monsterService.MoveMonsterToFightAsync(executionContext, monsterId, request.FightId, cancellationToken);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{MonsterId}/modifiers")]
    public async Task<CreatedActionResult<ActiveStatsModifier>> PostAddModifierAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterId,
        ActiveStatsModifier statsModifier,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var modifier = await monsterService.AddModifierAsync(executionContext, monsterId, statsModifier, cancellationToken);
            return modifier;
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpDelete("{MonsterId}/modifiers/{ModifierId}")]
    public async Task<CreatedActionResult<ActiveStatsModifier>> DeleteModifierAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterId,
        [FromRoute] int modifierId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await monsterService.RemoveModifierAsync(executionContext, monsterId, modifierId, cancellationToken);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (MonsterModifierNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPut("{MonsterId:int:min(1)}/data")]
    public async Task<CreatedActionResult<ItemResponse>> PutMonsterDataAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterId,
        MonsterData request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await monsterService.UpdateMonsterDataAsync(executionContext, monsterId, request, cancellationToken);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPut("{MonsterId:int:min(1)}/target")]
    public async Task<CreatedActionResult<ItemResponse>> PutMonsterTargetAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterId,
        TargetRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await monsterService.UpdateMonsterTargetAsync(executionContext, monsterId, request, cancellationToken);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (TargetNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
    }

    [HttpPatch("{MonsterId:int:min(1)}")]
    public async Task<CreatedActionResult<ItemResponse>> PatchMonsterAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterId,
        PatchMonsterRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await monsterService.UpdateMonsterAsync(executionContext, monsterId, request, cancellationToken);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{MonsterId:int:min(1)}/items")]
    public async Task<CreatedActionResult<ItemResponse>> PostAddItemToMonsterInventoryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterId,
        CreateItemRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var item = await monsterService.AddItemToMonsterAsync(executionContext, monsterId, request, cancellationToken);
            return itemMapper.ToResponse(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (ItemTemplateNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
    }

    [HttpPost("{MonsterId:int:min(1)}/addRandomItem")]
    public async Task<CreatedActionResult<ItemResponse>> PostAddRandomItemToMonsterInventoryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterId,
        CreateRandomItemRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var item = await monsterService.AddRandomItemToMonsterAsync(executionContext, monsterId, request, cancellationToken);
            return itemMapper.ToResponse(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (ItemTemplateSubCategoryNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
    }
}