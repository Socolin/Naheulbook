using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Item;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/itemTypes")]
[ApiController]
public class ItemTypesController(
    IItemTemplateMapper itemTemplateMapper,
    IItemTypeService itemTypeService
)
{
    [HttpGet]
    public async Task<ActionResult<List<ItemTypeResponse>>> GetAllItemTypesAsync(CancellationToken cancellationToken)
    {
        var itemTypes = await itemTypeService.GetAllItemTypesAsync(cancellationToken);
        return itemTypes.Select(itemTemplateMapper.ToResponse).ToList();
    }
}