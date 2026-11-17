using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Skill;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/skills")]
[ApiController]
public class SkillsController(ISkillService skillService, ISkillMapper skillMapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SkillResponse>>> GetAsync()
    {
        var skills = await skillService.GetSkillsAsync();

        return skills.Select(skillMapper.ToResponse).ToList();
    }
}