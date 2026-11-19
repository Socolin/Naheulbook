using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Features.Talent;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/characters/{CharacterId:int:min(1)}/talents")]
[ApiController]
public class CharacterTalentsController(
    ICharacterTalentService characterTalentService,
    ITalentMapper talentMapper
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<TalentResponse>> PostAddTalentAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromBody] AddCharacterTalentRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var talent = await characterTalentService.AddTalentAsync(executionContext, characterId, request, cancellationToken);
            return talentMapper.ToResponse(talent);
        }
        catch (Exception ex) when (TryGetHttpError(ex, out var httpError))
        {
            throw httpError;
        }
    }

    [HttpDelete("{TalentId:guid}")]
    public async Task<ActionResult> DeleteRemoveTalentAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int characterId,
        [FromRoute] Guid talentId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await characterTalentService.RemoveTalentAsync(executionContext, characterId, talentId, cancellationToken);
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
            TalentNotFoundException or TalentAlreadyKnownException => StatusCodes.Status400BadRequest,
            ForbiddenAccessException => StatusCodes.Status403Forbidden,
            CharacterNotFoundException or CharacterTalentNotFoundException => StatusCodes.Status404NotFound,
            _ => null,
        };
        httpError = statusCode.HasValue ? new HttpErrorException(statusCode.Value, ex) : null!;
        return statusCode.HasValue;
    }
}
