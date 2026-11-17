using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/monsterTraits")]
[ApiController]
public class MonsterTraitsController(IMonsterTraitService monsterTraitService, IMonsterTemplateMapper monsterTemplateMapper)
{
    [HttpGet]
    public async Task<ActionResult<List<MonsterTraitResponse>>> GetMonsterTraitsAsync(CancellationToken cancellationToken)
    {
        var monsterTraits = await monsterTraitService.GetMonsterTraitsAsync(cancellationToken);

        return monsterTraits.Select(monsterTemplateMapper.ToResponse).ToList();
    }
}