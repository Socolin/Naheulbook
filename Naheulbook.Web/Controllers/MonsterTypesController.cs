using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;
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
        public async Task<ActionResult<List<MonsterTypeResponse>>> GetMonsterTypesAsync()
        {
            var monsterTypes = await _monsterTypeService.GetMonsterTypesWithCategoriesAsync();
            return _mapper.Map<List<MonsterTypeResponse>>(monsterTypes);
        }


        [HttpPost]
        public async Task<CreatedActionResult<MonsterTypeResponse>> PostCreateMonsterTypeAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateMonsterTypeRequest request
        )
        {
            try
            {
                var monsterType = await _monsterTypeService.CreateMonsterTypeAsync(executionContext, request);
                return _mapper.Map<MonsterTypeResponse>(monsterType);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
        }

        [HttpPost("{MonsterTypeId}/categories")]
        public async Task<CreatedActionResult<MonsterCategoryResponse>> PostCreateMonsterCategoryAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int monsterTypeId,
            CreateMonsterCategoryRequest request
        )
        {
            try
            {
                var monsterCategory = await _monsterTypeService.CreateMonsterCategoryAsync(executionContext, monsterTypeId, request);
                return _mapper.Map<MonsterCategoryResponse>(monsterCategory);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
        }
    }
}