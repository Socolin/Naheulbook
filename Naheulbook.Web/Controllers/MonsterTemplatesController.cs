using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Filters;
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

        [HttpPost]
        [ServiceFilter(typeof(JwtAuthorizationFilter))]
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