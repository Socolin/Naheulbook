using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/items")]
    [ApiController]
    public class ItemsController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IMapper _mapper;

        public ItemsController(IItemService itemTemplate, IMapper mapper)
        {
            _itemService = itemTemplate;
            _mapper = mapper;
        }

        [HttpPut("{ItemId}/data")]
        public async Task<ActionResult<ItemPartialResponse>> PutEditItemDataAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int itemId,
            ItemData itemData
        )
        {
            try
            {
                var item = await _itemService.UpdateItemDataAsync(executionContext, itemId, itemData);

                return _mapper.Map<ItemPartialResponse>(item);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (ItemNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpDelete("{ItemId}")]
        public async Task<ActionResult<ItemPartialResponse>> DeleteItemAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int itemId
        )
        {
            try
            {
                await _itemService.DeleteItemAsync(executionContext, itemId);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (ItemNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPost("{ItemId}/useCharge")]
        public async Task<ActionResult<ItemPartialResponse>> PostUseChargeAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int itemId,
            [FromBody] UseChargeItemRequest request
        )
        {
            try
            {
                var item = await _itemService.UseChargeAsync(executionContext, itemId, request);
                return _mapper.Map<ItemPartialResponse>(item);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (ItemNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPut("{ItemId}/modifiers")]
        public async Task<ActionResult<ItemPartialResponse>> PutEditItemModifiersAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int itemId,
            IList<ActiveStatsModifier> itemModifiers
        )
        {
            try
            {
                var item = await _itemService.UpdateItemModifiersAsync(executionContext, itemId, itemModifiers);

                return _mapper.Map<ItemPartialResponse>(item);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (ItemNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPost("{ItemId}/equip")]
        public async Task<ActionResult<ItemPartialResponse>> PostEquipItemAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int itemId,
            EquipItemRequest request
        )
        {
            try
            {
                var item = await _itemService.EquipItemAsync(executionContext, itemId, request);

                return _mapper.Map<ItemPartialResponse>(item);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (ItemNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPut("{ItemId}/container")]
        public async Task<ActionResult<ItemPartialResponse>> PutChangeItemContainerAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int itemId,
            ChangeItemContainerRequest request
        )
        {
            try
            {
                var item = await _itemService.ChangeItemContainerAsync(executionContext, itemId, request);

                return _mapper.Map<ItemPartialResponse>(item);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (ItemNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPost("{ItemId}/take")]
        public async Task<ActionResult<TakeItemResponse>> PostTakeItemAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int itemId,
            TakeItemRequest request
        )
        {
            try
            {
                var (takenItem, remainingQuantity) = await _itemService.TakeItemAsync(executionContext, itemId, request);

                return new TakeItemResponse
                {
                    RemainingQuantity = remainingQuantity,
                    TakenItem = _mapper.Map<ItemResponse>(takenItem)
                };
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (ItemNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }


        [HttpPost("{ItemId}/give")]
        public async Task<ActionResult<GiveItemResponse>> PostGiveItemAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int itemId,
            TakeItemRequest request
        )
        {
            try
            {
                var remainingQuantity = await _itemService.GiveItemAsync(executionContext, itemId, request);

                return new GiveItemResponse
                {
                    RemainingQuantity = remainingQuantity
                };
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (ItemNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }
    }
}