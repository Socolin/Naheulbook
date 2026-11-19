using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Features.Mastery;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/characters/{CharacterId:int:min(1)}/masteries")]
[ApiController]
public class CharacterMasteriesController(
    ICharacterMasteryService characterMasteryService,
    IMasteryMapper masteryMapper
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<MasteryResponse>> PostAddMasteryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromBody] AddCharacterMasteryRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var mastery = await characterMasteryService.AddMasteryAsync(executionContext, characterId, request, cancellationToken);
            return masteryMapper.ToResponse(mastery);
        }
        catch (Exception ex) when (TryGetHttpError(ex, out var httpError))
        {
            throw httpError;
        }
    }

    [HttpDelete("{MasteryId:guid}")]
    public async Task<ActionResult> DeleteRemoveMasteryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] Guid masteryId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterMasteryService.RemoveMasteryAsync(executionContext, characterId, masteryId, cancellationToken);
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
            MasteryNotFoundException or MasteryAlreadyKnownException => StatusCodes.Status400BadRequest,
            ForbiddenAccessException => StatusCodes.Status403Forbidden,
            CharacterNotFoundException or CharacterMasteryNotFoundException => StatusCodes.Status404NotFound,
            _ => null,
        };
        httpError = statusCode.HasValue ? new HttpErrorException(statusCode.Value, ex) : null!;
        return statusCode.HasValue;
    }
}
