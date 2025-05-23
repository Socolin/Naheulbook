﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Skill;
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