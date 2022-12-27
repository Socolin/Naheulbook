using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/itemTypes")]
[ApiController]
public class ItemTypesController
{
    private readonly IMapper _mapper;
    private readonly IItemTypeService _itemTypeService;

    public ItemTypesController(
        IMapper mapper,
        IItemTypeService itemTypeService
    )
    {
        _mapper = mapper;
        _itemTypeService = itemTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ItemTypeResponse>>> GetAllItemTypesAsync()
    {
        var itemTypes = await _itemTypeService.GetAllItemTypesAsync();
        return _mapper.Map<List<ItemTypeResponse>>(itemTypes);
    }
}