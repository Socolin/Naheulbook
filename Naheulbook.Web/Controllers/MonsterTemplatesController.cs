using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/monsterTemplates")]
    [ApiController]
    public class MonsterTemplatesController
    {
        private readonly IMonsterTemplateService _monsterTemplateService;
        private readonly IMapper _mapper;

        public MonsterTemplatesController(IMapper mapper, IMonsterTemplateService monsterTemplateService)
        {
            _mapper = mapper;
            _monsterTemplateService = monsterTemplateService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MonsterTemplateResponse>>> GetMonsterList()
        {
            var monsters = await _monsterTemplateService.GetAllMonstersAsync();
            return _mapper.Map<List<MonsterTemplateResponse>>(monsters);
        }

        [HttpPost]
        public async Task<ActionResult<MonsterTemplateResponse>> Post(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateMonsterTemplateRequest request
        )
        {
            var createdMonster = await _monsterTemplateService.CreateMonsterTemplate(executionContext, request);

            var result = _mapper.Map<MonsterTemplateResponse>(createdMonster);
            return new JsonResult(result) {StatusCode = (int) HttpStatusCode.Created};
        }
    }
}