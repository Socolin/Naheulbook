using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Models.Backup;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/characters")]
[ApiController]
public class CharactersController(
    ICharacterService characterService,
    IMapper mapper,
    ICharacterBackupService characterBackupService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CharacterSummaryResponse>>> GetCharactersListAsync(
        [FromServices] NaheulbookExecutionContext executionContext
    )
    {
        var characters = await characterService.GetCharacterListAsync(executionContext);
        return mapper.Map<List<CharacterSummaryResponse>>(characters);
    }

    [HttpGet("{CharacterId:int:min(1)}")]
    public async Task<ActionResult<CharacterResponse>> GetCharacterDetailsAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId
    )
    {
        try
        {
            var character = await characterService.LoadCharacterDetailsAsync(executionContext, characterId);

            if (executionContext.UserId == character.Group?.MasterId)
                return mapper.Map<CharacterFoGmResponse>(character);

            return mapper.Map<CharacterResponse>(character);
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
        [FromQuery] string filter
    )
    {
        var characters = await characterService.SearchCharactersAsync(filter);
        return mapper.Map<List<CharacterSearchResponse>>(characters);
    }

    [HttpPost]
    public async Task<CreatedActionResult<CreateCharacterResponse>> PostCreateCharacterAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateCharacterRequest request
    )
    {
        try
        {
            var character = await characterService.CreateCharacterAsync(executionContext, request);
            return mapper.Map<CreateCharacterResponse>(character);
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
        CreateCustomCharacterRequest request
    )
    {
        try
        {
            var character = await characterService.CreateCustomCharacterAsync(executionContext, request);
            return mapper.Map<CreateCharacterResponse>(character);
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
        CreateItemRequest request
    )
    {
        try
        {
            var item = await characterService.AddItemToCharacterAsync(executionContext, characterId, request);
            return mapper.Map<ItemResponse>(item);
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
        AddCharacterModifierRequest request
    )
    {
        try
        {
            var item = await characterService.AddModifiersAsync(executionContext, characterId, request);
            return mapper.Map<ActiveStatsModifier>(item);
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
        [FromRoute] int characterModifierId
    )
    {
        try
        {
            await characterService.DeleteModifiersAsync(executionContext, characterId, characterModifierId);
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
        [FromRoute] int characterModifierId
    )
    {
        try
        {
            var characterModifier = await characterService.ToggleModifiersAsync(executionContext, characterId, characterModifierId);
            return mapper.Map<ActiveStatsModifier>(characterModifier);
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
        [FromRoute] int characterId
    )
    {
        try
        {
            var loots = await characterService.GetCharacterLootsAsync(executionContext, characterId);
            return mapper.Map<List<LootResponse>>(loots);
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
        [FromRoute] int characterId
    )
    {
        try
        {
            return await characterBackupService.GetBackupCharacterAsync(executionContext, characterId);
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
        [FromQuery] int page
    )
    {
        try
        {
            var loots = await characterService.GetCharacterHistoryEntryAsync(executionContext, characterId, page);
            return mapper.Map<List<IHistoryEntryResponse>>(loots);
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
        PatchCharacterRequest request
    )
    {
        try
        {
            await characterService.UpdateCharacterAsync(executionContext, characterId, request);
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
        PutStatBonusAdRequest request
    )
    {
        try
        {
            await characterService.SetCharacterAdBonusStatAsync(executionContext, characterId, request);
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
        CharacterLevelUpRequest request
    )
    {
        try
        {
            var levelUpResult = await characterService.LevelUpCharacterAsync(executionContext, characterId, request);
            return mapper.Map<CharacterLevelUpResponse>(levelUpResult);
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
        CharacterAddJobRequest request
    )
    {
        try
        {
            await characterService.AddJobAsync(executionContext, characterId, request);
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
        CharacterRemoveJobRequest request
    )
    {
        try
        {
            await characterService.RemoveJobAsync(executionContext, characterId, request);
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
        [FromRoute] int characterId
    )
    {
        try
        {
            await characterService.QuitGroupAsync(executionContext, characterId);
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
}