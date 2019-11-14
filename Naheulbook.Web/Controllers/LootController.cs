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
    [Route("api/v2/loots")]
    [ApiController]
    public class LootController : ControllerBase
    {
        private readonly ILootService _lootService;
        private readonly IMapper _mapper;

        public LootController(
            ILootService lootService,
            IMapper mapper
        )
        {
            _lootService = lootService;
            _mapper = mapper;
        }

        [HttpPut("{LootId:int:min(1)}/visibility")]
        public async Task<IActionResult> PutLootVisibilityAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int lootId,
            PutLootVisibilityRequest request
        )
        {
            try
            {
                await _lootService.UpdateLootVisibilityAsync(executionContext, lootId, request);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (LootNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }

        [HttpDelete("{LootId:int:min(1)}")]
        public async Task<IActionResult> DeleteLootAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int lootId
        )
        {
            try
            {
                await _lootService.DeleteLootAsync(executionContext, lootId);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (LootNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
        }

        [HttpPost("{LootId:int:min(1)}/items")]
        public async Task<CreatedActionResult<ItemResponse>> PostAddItemToLootInventoryAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int lootId,
            CreateItemRequest request
        )
        {
            try
            {
                var item = await _lootService.AddItemToLootAsync(executionContext, lootId, request);
                return _mapper.Map<ItemResponse>(item);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (LootNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
            catch (ItemTemplateNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
            }
        }

        [HttpPost("{LootId:int:min(1)}/addRandomItem")]
        public async Task<CreatedActionResult<ItemResponse>> PostAddRandomItemToLootInventoryAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int lootId,
            CreateRandomItemRequest request
        )
        {
            try
            {
                var item = await _lootService.AddRandomItemToLootAsync(executionContext, lootId, request);
                return _mapper.Map<ItemResponse>(item);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
            catch (LootNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
            }
            catch (ItemTemplateSubCategoryNotFoundException ex)
            {
                throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
            }
        }
    }
}