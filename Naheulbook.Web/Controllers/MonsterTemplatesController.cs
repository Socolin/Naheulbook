using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
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
        public async Task<ActionResult<List<MonsterTemplateResponse>>> GetMonsterListAsync()
        {
            var monsters = await _monsterTemplateService.GetAllMonstersAsync();
            return _mapper.Map<List<MonsterTemplateResponse>>(monsters);
        }

        [HttpPost]
        public async Task<ActionResult<MonsterTemplateResponse>> PostAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            MonsterTemplateRequest request
        )
        {
            try
            {
                var createdMonster = await _monsterTemplateService.CreateMonsterTemplateAsync(executionContext, request);

                var result = _mapper.Map<MonsterTemplateResponse>(createdMonster);
                return new JsonResult(result) {StatusCode = StatusCodes.Status201Created};
            }
            catch (MonsterSubCategoryNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
        }

        [HttpPut("{MonsterTemplateId}")]
        public async Task<ActionResult<MonsterTemplateResponse>> PutAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int monsterTemplateId,
            MonsterTemplateRequest request
        )
        {
            try
            {
                var monster = await _monsterTemplateService.EditMonsterTemplateAsync(executionContext, monsterTemplateId, request);

                var result = _mapper.Map<MonsterTemplateResponse>(monster);
                return new JsonResult(result);
            }
            catch (MonsterSubCategoryNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
            }
            catch (MonsterTemplateNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<MonsterTemplateResponse>>> GetSearchMonsterTemplateAsync(
            [FromQuery] string filter,
            [FromQuery] int? monsterTypeId,
            [FromQuery] int? monsterSubCategoryId
        )
        {
            var monsterTemplates = await _monsterTemplateService.SearchMonsterAsync(filter, monsterTypeId, monsterSubCategoryId);
            return _mapper.Map<List<MonsterTemplateResponse>>(monsterTemplates);
        }
    }
}