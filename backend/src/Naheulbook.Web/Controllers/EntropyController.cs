using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Entropy;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

/// <summary>
/// Mankdebol calamities and Débilibeuk blessings (pack de l'entropie)
/// </summary>
[ApiController]
public class EntropyController(
    IEntropyService entropyService,
    IEntropyMapper entropyMapper
) : ControllerBase
{
    [HttpGet("api/v2/entropyTables")]
    public async Task<ActionResult<EntropyTablesResponse>> GetEntropyTablesAsync(
        CancellationToken cancellationToken
    )
    {
        var tables = await entropyService.GetEntropyTablesAsync(cancellationToken);
        return new EntropyTablesResponse
        {
            MankdebolCalamities = tables.MankdebolCalamities.Select(entropyMapper.ToResponse).ToList(),
            DebilibeukBlessings = tables.DebilibeukBlessings.Select(entropyMapper.ToResponse).ToList(),
        };
    }

    [HttpPost("api/v2/groups/{GroupId:int:min(1)}/mankdebolCalamities")]
    public Task<IActionResult> PostApplyMankdebolCalamityAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId,
        [FromBody] PostApplyEntropyEventRequest request,
        CancellationToken cancellationToken
    )
    {
        return ApplyEntropyEventAsync(executionContext, groupId, EntropyKind.MankdebolCalamity, request, cancellationToken);
    }

    [HttpPost("api/v2/groups/{GroupId:int:min(1)}/debilibeukBlessings")]
    public Task<IActionResult> PostApplyDebilibeukBlessingAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int groupId,
        [FromBody] PostApplyEntropyEventRequest request,
        CancellationToken cancellationToken
    )
    {
        return ApplyEntropyEventAsync(executionContext, groupId, EntropyKind.DebilibeukBlessing, request, cancellationToken);
    }

    private async Task<IActionResult> ApplyEntropyEventAsync(
        NaheulbookExecutionContext executionContext,
        int groupId,
        EntropyKind kind,
        PostApplyEntropyEventRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await entropyService.ApplyEntropyEventAsync(executionContext, groupId, kind, request, cancellationToken);
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
            EntropyEventNotFoundException or CharacterNotInGroupException => StatusCodes.Status400BadRequest,
            ForbiddenAccessException => StatusCodes.Status403Forbidden,
            GroupNotFoundException => StatusCodes.Status404NotFound,
            _ => null,
        };
        httpError = statusCode.HasValue ? new HttpErrorException(statusCode.Value, ex) : null!;
        return statusCode.HasValue;
    }
}
