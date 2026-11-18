using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Aptitude;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Character.Backup;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Job;
using Naheulbook.Core.Features.Loot;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Features.SpecialAttack;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/characters")]
[ApiController]
public class CharactersController(
    ICharacterService characterService,
    IAptitudeMapper aptitudeMapper,
    ICharacterMapper characterMapper,
    ICharacterModifierMapper characterModifierMapper,
    IItemMapper itemMapper,
    ILootMapper lootMapper,
    ICharacterBackupService characterBackupService,
    ICharacterAptitudeService characterAptitudeService,
    ICharacterSpecialAttackService characterSpecialAttackService,
    ISpecialAttackMapper specialAttackMapper
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CharacterSummaryResponse>>> GetCharactersListAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CancellationToken cancellationToken
    )
    {
        var characters = await characterService.GetCharacterListAsync(executionContext, cancellationToken);
        return characters.Select(characterMapper.ToSummaryResponse).ToList();
    }

    [HttpGet("{CharacterId:int:min(1)}")]
    public async Task<ActionResult<CharacterResponse>> GetCharacterDetailsAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var character = await characterService.LoadCharacterDetailsAsync(executionContext, characterId, cancellationToken);

            if (executionContext.UserId == character.Group?.MasterId)
                return characterMapper.ToGmResponse(character);

            return characterMapper.ToResponse(character);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<CharacterSearchResponse>>> GetSearchCharacterAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromQuery] string filter,
        CancellationToken cancellationToken
    )
    {
        var characters = await characterService.SearchCharactersAsync(filter, cancellationToken);
        return characters.Select(characterMapper.ToSearchResponse).ToList();
    }

    [HttpPost]
    public async Task<CreatedActionResult<CreateCharacterResponse>> PostCreateCharacterAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateCharacterRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var character = await characterService.CreateCharacterAsync(executionContext, request, cancellationToken);
            return characterMapper.ToCreateResponse(character);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (GroupNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
    }

    [HttpPost("custom")]
    public async Task<CreatedActionResult<CreateCharacterResponse>> PostCreateCustomCharacterAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateCustomCharacterRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var character = await characterService.CreateCustomCharacterAsync(executionContext, request, cancellationToken);
            return characterMapper.ToCreateResponse(character);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (GroupNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
    }

    [HttpPost("{CharacterId:int:min(1)}/items")]
    public async Task<CreatedActionResult<ItemResponse>> PostAddItemToCharacterInventoryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        CreateItemRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var item = await characterService.AddItemToCharacterAsync(executionContext, characterId, request, cancellationToken);
            return itemMapper.ToResponse(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (ItemTemplateNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
    }

    [HttpPost("{CharacterId:int:min(1)}/modifiers")]
    public async Task<CreatedActionResult<ActiveStatsModifier>> PostAddModifiersAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        AddCharacterModifierRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var item = await characterService.AddModifiersAsync(executionContext, characterId, request, cancellationToken);
            return characterModifierMapper.ToResponse(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpDelete("{CharacterId:int:min(1)}/modifiers/{CharacterModifierId}")]
    public async Task<IActionResult> DeleteModifiersAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] int characterModifierId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterService.DeleteModifiersAsync(executionContext, characterId, characterModifierId, cancellationToken);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (CharacterModifierNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{CharacterId:int:min(1)}/modifiers/{CharacterModifierId}/toggle")]
    public async Task<ActionResult<ActiveStatsModifier>> PostToggleModifiersAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] int characterModifierId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var characterModifier = await characterService.ToggleModifiersAsync(executionContext, characterId, characterModifierId, cancellationToken);
            return characterModifierMapper.ToResponse(characterModifier);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (CharacterModifierNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (CharacterModifierNotReusableException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
    }

    [HttpGet("{CharacterId:int:min(1)}/loots")]
    public async Task<ActionResult<List<LootResponse>>> GetCharacterLootsAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var loots = await characterService.GetCharacterLootsAsync(executionContext, characterId, cancellationToken);
            return loots.Select(lootMapper.ToResponse).ToList();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpGet("{CharacterId:int:min(1)}/backup")]
    public async Task<ActionResult<BackupCharacter>> GetBackupCharacterAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await characterBackupService.GetBackupCharacterAsync(executionContext, characterId, cancellationToken);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpGet("{CharacterId:int:min(1)}/history")]
    public async Task<ActionResult<List<IHistoryEntryResponse>>> GetCharacterHistoryEntryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromQuery] int page,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var loots = await characterService.GetCharacterHistoryEntryAsync(executionContext, characterId, page, cancellationToken);
            return loots.Select(characterMapper.ToResponse).ToList();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPatch("{CharacterId:int:min(1)}")]
    public async Task<IActionResult> PatchCharacterAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        PatchCharacterRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterService.UpdateCharacterAsync(executionContext, characterId, request, cancellationToken);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPut("{CharacterId:int:min(1)}/statBonusAd")]
    public async Task<IActionResult> PutStatBonusAdAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        PutStatBonusAdRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterService.SetCharacterAdBonusStatAsync(executionContext, characterId, request, cancellationToken);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{CharacterId:int:min(1)}/levelUp")]
    public async Task<ActionResult<CharacterLevelUpResponse>> PostCharacterLevelUpAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        CharacterLevelUpRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var levelUpResult = await characterService.LevelUpCharacterAsync(executionContext, characterId, request, cancellationToken);
            return characterMapper.ToResponse(levelUpResult);
        }
        catch (SpecialityNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (InvalidTargetLevelUpRequestedException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{CharacterId:int:min(1)}/addJob")]
    public async Task<ActionResult<CharacterAddJobResponse>> PostCharacterAddJobAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        CharacterAddJobRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterService.AddJobAsync(executionContext, characterId, request, cancellationToken);
            return new CharacterAddJobResponse {JobId = request.JobId};
        }
        catch (CharacterAlreadyKnowThisJobException ex)
        {
            throw new HttpErrorException(StatusCodes.Status409Conflict, ex);
        }
        catch (JobNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{CharacterId:int:min(1)}/removeJob")]
    public async Task<ActionResult<CharacterRemoveJobResponse>> PostCharacterRemoveJobAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        CharacterRemoveJobRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterService.RemoveJobAsync(executionContext, characterId, request, cancellationToken);
            return new CharacterRemoveJobResponse {JobId = request.JobId};
        }
        catch (CharacterAlreadyKnowThisJobException ex)
        {
            throw new HttpErrorException(StatusCodes.Status409Conflict, ex);
        }
        catch (JobNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }


    [HttpPost("{CharacterId:int:min(1)}/quitGroup")]
    public async Task<ActionResult<CharacterLevelUpResponse>> PostCharacterQuitGroupAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterService.QuitGroupAsync(executionContext, characterId, cancellationToken);
            return NoContent();
        }
        catch (CharacterNotInAGroupException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{CharacterId:int:min(1)}/aptitudes")]
    public async Task<ActionResult<CharacterAddAptitudeResponse>> PostCharacterAddAptitudeAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromBody] CharacterAddAptitudeRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var characterAptitude = await characterAptitudeService.AddAptitudeAsync(executionContext, characterId, request, cancellationToken);
            return aptitudeMapper.ToAddResponse(characterAptitude);
        }
        catch (AptitudeNotAvailableForOriginException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (AptitudeNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpDelete("{CharacterId:int:min(1)}/aptitudes/{AptitudeId:guid}")]
    public async Task<ActionResult<CharacterAddAptitudeResponse>> PostCharacterDeleteAptitudeAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] Guid aptitudeId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterAptitudeService.RemoveAptitudeAsync(executionContext, characterId, aptitudeId, cancellationToken);
            return NoContent();
        }
        catch (AptitudeNotAvailableForOriginException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (AptitudeNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPut("{CharacterId:int:min(1)}/aptitudes/{AptitudeId:guid}")]
    public async Task<ActionResult<CharacterAddAptitudeResponse>> PatchCharacterAptitudeAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] Guid aptitudeId,
        [FromBody] UpdateCharacterAptitudeRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterAptitudeService.UpdateCharacterAptitudeAsync(executionContext, characterId, aptitudeId, request, cancellationToken);
            return NoContent();
        }
        catch (AptitudeNotAvailableForOriginException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (AptitudeNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (CharacterNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{CharacterId:int:min(1)}/specialAttacks")]
    public async Task<ActionResult<CharacterSpecialAttackResponse>> PostCharacterAddSpecialAttackAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromBody] AddCharacterSpecialAttackRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var characterSpecialAttack = await characterSpecialAttackService.AddSpecialAttackAsync(executionContext, characterId, request, cancellationToken);
            return specialAttackMapper.ToResponse(characterSpecialAttack);
        }
        catch (Exception ex) when (TryGetSpecialAttackHttpError(ex, out var httpError))
        {
            throw httpError;
        }
    }

    [HttpDelete("{CharacterId:int:min(1)}/specialAttacks/{SpecialAttackId:guid}")]
    public async Task<ActionResult> DeleteCharacterSpecialAttackAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] Guid specialAttackId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterSpecialAttackService.RemoveSpecialAttackAsync(executionContext, characterId, specialAttackId, cancellationToken);
            return NoContent();
        }
        catch (Exception ex) when (TryGetSpecialAttackHttpError(ex, out var httpError))
        {
            throw httpError;
        }
    }

    [HttpPost("{CharacterId:int:min(1)}/specialAttacks/{SpecialAttackId:guid}/use")]
    public async Task<ActionResult<UseCharacterSpecialAttackResponse>> PostCharacterUseSpecialAttackAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] Guid specialAttackId,
        [FromBody] UseCharacterSpecialAttackRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await characterSpecialAttackService.UseSpecialAttackAsync(executionContext, characterId, specialAttackId, request, cancellationToken);
            return specialAttackMapper.ToResponse(result);
        }
        catch (Exception ex) when (TryGetSpecialAttackHttpError(ex, out var httpError))
        {
            throw httpError;
        }
    }

    [HttpPut("{CharacterId:int:min(1)}/specialAttacks/{SpecialAttackId:guid}")]
    public async Task<ActionResult<CharacterSpecialAttackResponse>> PutCharacterSpecialAttackAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] Guid specialAttackId,
        [FromBody] UpdateCharacterSpecialAttackRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var characterSpecialAttack = await characterSpecialAttackService.UpdateSpecialAttackAsync(executionContext, characterId, specialAttackId, request, cancellationToken);
            return specialAttackMapper.ToResponse(characterSpecialAttack);
        }
        catch (Exception ex) when (TryGetSpecialAttackHttpError(ex, out var httpError))
        {
            throw httpError;
        }
    }

    private static bool TryGetSpecialAttackHttpError(Exception ex, out HttpErrorException httpError)
    {
        int? statusCode = ex switch
        {
            SpecialAttackNotFoundException or SpecialAttackAlreadyKnownException or SpecialAttackNotAvailableException => StatusCodes.Status400BadRequest,
            ForbiddenAccessException => StatusCodes.Status403Forbidden,
            CharacterNotFoundException or CharacterSpecialAttackNotFoundException => StatusCodes.Status404NotFound,
            _ => null,
        };
        httpError = statusCode.HasValue ? new HttpErrorException(statusCode.Value, ex) : null!;
        return statusCode.HasValue;
    }
}
