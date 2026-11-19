using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Features.Handicap;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/characters/{CharacterId:int:min(1)}/handicaps")]
[ApiController]
public class CharacterHandicapsController(
    ICharacterHandicapService characterHandicapService,
    IHandicapMapper handicapMapper
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<HandicapResponse>> PostAddHandicapAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromBody] AddCharacterHandicapRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var handicap = await characterHandicapService.AddHandicapAsync(executionContext, characterId, request, cancellationToken);
            return handicapMapper.ToResponse(handicap);
        }
        catch (Exception ex) when (TryGetHttpError(ex, out var httpError))
        {
            throw httpError;
        }
    }

    [HttpDelete("{HandicapId:guid}")]
    public async Task<ActionResult> DeleteRemoveHandicapAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] Guid handicapId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterHandicapService.RemoveHandicapAsync(executionContext, characterId, handicapId, cancellationToken);
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
            HandicapNotFoundException or HandicapAlreadyKnownException => StatusCodes.Status400BadRequest,
            ForbiddenAccessException => StatusCodes.Status403Forbidden,
            CharacterNotFoundException or CharacterHandicapNotFoundException => StatusCodes.Status404NotFound,
            _ => null,
        };
        httpError = statusCode.HasValue ? new HttpErrorException(statusCode.Value, ex) : null!;
        return statusCode.HasValue;
    }
}
