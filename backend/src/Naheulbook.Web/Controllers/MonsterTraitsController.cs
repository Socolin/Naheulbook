using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/monsterTraits")]
[ApiController]
public class MonsterTraitsController(IMonsterTraitService monsterTraitService, IMonsterTemplateMapper monsterTemplateMapper)
{
    [HttpGet]
    public async Task<ActionResult<List<MonsterTraitResponse>>> GetMonsterTraitsAsync()
    {
        var monsterTraits = await monsterTraitService.GetMonsterTraitsAsync();

        return monsterTraits.Select(monsterTemplateMapper.ToResponse).ToList();
    }
}