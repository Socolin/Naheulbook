using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Talent;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/talentGroups")]
[ApiController]
public class TalentGroupController(
    ITalentService talentService,
    ITalentMapper talentMapper
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TalentGroupsResponse>> GetTalentGroupsAsync(
        CancellationToken cancellationToken
    )
    {
        var talentGroups = await talentService.GetTalentGroupsAsync(cancellationToken);
        return new TalentGroupsResponse
        {
            TalentGroups = talentGroups.Select(talentMapper.ToResponse).ToList(),
        };
    }
}
