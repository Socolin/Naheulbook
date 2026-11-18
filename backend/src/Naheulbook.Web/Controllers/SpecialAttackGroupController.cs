using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.SpecialAttack;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/specialAttackGroups")]
[ApiController]
public class SpecialAttackGroupController(
    ISpecialAttackService specialAttackService,
    ISpecialAttackMapper specialAttackMapper
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SpecialAttackGroupsResponse>> GetSpecialAttackGroupsAsync(
        CancellationToken cancellationToken
    )
    {
        var specialAttackGroups = await specialAttackService.GetSpecialAttackGroupsAsync(cancellationToken);
        return new SpecialAttackGroupsResponse
        {
            SpecialAttackGroups = specialAttackGroups.Select(specialAttackMapper.ToResponse).ToList(),
        };
    }
}
