using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/monsterTraits")]
[ApiController]
public class MonsterTraitsController(IMonsterTraitService monsterTraitService, IMapper mapper)
{
    [HttpGet]
    public async Task<ActionResult<List<MonsterTraitResponse>>> GetMonsterTraitsAsync()
    {
        var monsterTraits = await monsterTraitService.GetMonsterTraitsAsync();

        return mapper.Map<List<MonsterTraitResponse>>(monsterTraits);
    }
}