using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Features.Spell;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/characters/{CharacterId:int:min(1)}/spellBooks")]
[ApiController]
public class CharacterSpellBooksController(
    ICharacterSpellBookService characterSpellBookService,
    ISpellMapper spellMapper
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CharacterSpellBookResponse>> PostAddSpellBookAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromBody] AddCharacterSpellBookRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var characterSpellBook = await characterSpellBookService.AddSpellBookAsync(executionContext, characterId, request, cancellationToken);
            return spellMapper.ToResponse(characterSpellBook);
        }
        catch (Exception ex) when (TryGetHttpError(ex, out var httpError))
        {
            throw httpError;
        }
    }

    [HttpPut("{SpellBookId:guid}")]
    public async Task<ActionResult<CharacterSpellBookResponse>> PutUpdateSpellBookAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] Guid spellBookId,
        [FromBody] UpdateCharacterSpellBookRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var characterSpellBook = await characterSpellBookService.UpdateSpellBookAsync(executionContext, characterId, spellBookId, request, cancellationToken);
            return spellMapper.ToResponse(characterSpellBook);
        }
        catch (Exception ex) when (TryGetHttpError(ex, out var httpError))
        {
            throw httpError;
        }
    }

    [HttpDelete("{SpellBookId:guid}")]
    public async Task<ActionResult> DeleteRemoveSpellBookAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] Guid spellBookId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterSpellBookService.RemoveSpellBookAsync(executionContext, characterId, spellBookId, cancellationToken);
            return NoContent();
        }
        catch (Exception ex) when (TryGetHttpError(ex, out var httpError))
        {
            throw httpError;
        }
    }

    private static bool TryGetHttpError(Exception ex, out HttpErrorException httpError)
    {
        int? statusCode = ex switch
        {
            SpellBookNotFoundException or SpellBookAlreadyLinkedException or InvalidSpellBookMaxLevelException => StatusCodes.Status400BadRequest,
            ForbiddenAccessException => StatusCodes.Status403Forbidden,
            CharacterNotFoundException or CharacterSpellBookNotFoundException => StatusCodes.Status404NotFound,
            _ => null,
        };
        httpError = statusCode.HasValue ? new HttpErrorException(statusCode.Value, ex) : null!;
        return statusCode.HasValue;
    }
}
