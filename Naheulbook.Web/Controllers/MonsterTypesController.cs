using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/monsterTypes")]
    [ApiController]
    public class MonsterTypesController
    {
        private readonly IMonsterTypeService _monsterTypeService;
        private readonly IMapper _mapper;

        public MonsterTypesController(IMapper mapper, IMonsterTypeService monsterTypeService)
        {
            _mapper = mapper;
            _monsterTypeService = monsterTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<MonsterTypeResponse>> GetMonsterTypes()
        {
            var createdMonster = await _monsterTypeService.GetMonsterTypesWithCategoriesAsync();

            var result = _mapper.Map<List<MonsterTypeResponse>>(createdMonster);

            return new JsonResult(result);
        }
    }
}