using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/monsterTraits")]
[ApiController]
public class MonsterTraitsController
{
    private readonly IMonsterTraitService _monsterTraitService;
    private readonly IMapper _mapper;

    public MonsterTraitsController(IMonsterTraitService monsterTraitService, IMapper mapper)
    {
        _monsterTraitService = monsterTraitService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<MonsterTraitResponse>>> GetMonsterTraitsAsync()
    {
        var monsterTraits = await _monsterTraitService.GetMonsterTraitsAsync();

        return _mapper.Map<List<MonsterTraitResponse>>(monsterTraits);
    }
}