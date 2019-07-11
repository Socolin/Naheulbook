using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [ApiController]
    [Route("api/v2/monsters")]
    public class MonsterController : Controller
    {
        private readonly IMonsterService _monsterService;
        private readonly IMapper _mapper;

        public MonsterController(
            IMonsterService monsterService,
            IMapper mapper
        )
        {
            _monsterService = monsterService;
            _mapper = mapper;
        }

        [HttpDelete("{MonsterId}")]
        public async Task<CreatedActionResult<ActiveStatsModifier>> DeleteMonsterAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int monsterId
        )
        {
            try
            {
                await _monsterService.DeleteMonsterAsync(executionContext, monsterId);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (MonsterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPost("{MonsterId}/kill")]
        public async Task<CreatedActionResult<ActiveStatsModifier>> PostKillMonsterAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int monsterId
        )
        {
            try
            {
                await _monsterService.KillMonsterAsync(executionContext, monsterId);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (MonsterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPost("{MonsterId}/modifiers")]
        public async Task<CreatedActionResult<ActiveStatsModifier>> PostAddModifier(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int monsterId,
            ActiveStatsModifier statsModifier
        )
        {
            try
            {
                var modifier = await _monsterService.AddModifierAsync(executionContext, monsterId, statsModifier);
                return modifier;
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (MonsterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpDelete("{MonsterId}/modifiers/{ModifierId}")]
        public async Task<CreatedActionResult<ActiveStatsModifier>> DeleteModifier(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int monsterId,
            [FromRoute] int modifierId
        )
        {
            try
            {
                await _monsterService.RemoveModifierAsync(executionContext, monsterId, modifierId);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (MonsterModifierNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
            catch (MonsterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPost("{MonsterId:int:min(1)}/items")]
        public async Task<CreatedActionResult<ItemResponse>> PostAddItemToMonsterInventory(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int monsterId,
            CreateItemRequest request
        )
        {
            try
            {
                var item = await _monsterService.AddItemToMonsterAsync(executionContext, monsterId, request);
                return _mapper.Map<ItemResponse>(item);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (MonsterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
            catch (ItemTemplateNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}