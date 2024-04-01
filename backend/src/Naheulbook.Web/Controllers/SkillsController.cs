using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/skills")]
[ApiController]
public class SkillsController(ISkillService skillService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SkillResponse>>> GetAsync()
    {
        var skills = await skillService.GetSkillsAsync();

        return mapper.Map<List<SkillResponse>>(skills);
    }
}