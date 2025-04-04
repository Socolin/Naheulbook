using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Item;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/itemTypes")]
[ApiController]
public class ItemTypesController(
    IMapper mapper,
    IItemTypeService itemTypeService
)
{
    [HttpGet]
    public async Task<ActionResult<List<ItemTypeResponse>>> GetAllItemTypesAsync()
    {
        var itemTypes = await itemTypeService.GetAllItemTypesAsync();
        return mapper.Map<List<ItemTypeResponse>>(itemTypes);
    }
}