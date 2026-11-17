using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/items")]
[ApiController]
public class ItemsController(IItemService itemTemplate, IItemMapper itemMapper) : ControllerBase
{
    [HttpPut("{ItemId}/data")]
    public async Task<ActionResult<ItemPartialResponse>> PutEditItemDataAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        ItemData itemData,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var item = await itemTemplate.UpdateItemDataAsync(executionContext, itemId, itemData, cancellationToken);

            return itemMapper.ToPartialResponse(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpDelete("{ItemId}")]
    public async Task<ActionResult<ItemPartialResponse>> DeleteItemAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await itemTemplate.DeleteItemAsync(executionContext, itemId, cancellationToken);
            return NoContent();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{ItemId}/useCharge")]
    public async Task<ActionResult<ItemPartialResponse>> PostUseChargeAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        [FromBody] UseChargeItemRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var item = await itemTemplate.UseChargeAsync(executionContext, itemId, request, cancellationToken);
            return itemMapper.ToPartialResponse(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPut("{ItemId}/modifiers")]
    public async Task<ActionResult<ItemPartialResponse>> PutEditItemModifiersAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        IList<ActiveStatsModifier> itemModifiers,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var item = await itemTemplate.UpdateItemModifiersAsync(executionContext, itemId, itemModifiers, cancellationToken);

            return itemMapper.ToPartialResponse(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{ItemId}/equip")]
    public async Task<ActionResult<ItemPartialResponse>> PostEquipItemAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        EquipItemRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var item = await itemTemplate.EquipItemAsync(executionContext, itemId, request, cancellationToken);

            return itemMapper.ToPartialResponse(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPut("{ItemId}/container")]
    public async Task<ActionResult<ItemPartialResponse>> PutChangeItemContainerAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        ChangeItemContainerRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var item = await itemTemplate.ChangeItemContainerAsync(executionContext, itemId, request, cancellationToken);

            return itemMapper.ToPartialResponse(item);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{ItemId}/take")]
    public async Task<ActionResult<TakeItemResponse>> PostTakeItemAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        TakeItemRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var (takenItem, remainingQuantity) = await itemTemplate.TakeItemAsync(executionContext, itemId, request, cancellationToken);

            return new TakeItemResponse
            {
                RemainingQuantity = remainingQuantity,
                TakenItem = itemMapper.ToResponse(takenItem),
            };
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }


    [HttpPost("{ItemId}/give")]
    public async Task<ActionResult<GiveItemResponse>> PostGiveItemAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int itemId,
        GiveItemRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var remainingQuantity = await itemTemplate.GiveItemAsync(executionContext, itemId, request, cancellationToken);

            return new GiveItemResponse
            {
                RemainingQuantity = remainingQuantity,
            };
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (ItemNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }
}