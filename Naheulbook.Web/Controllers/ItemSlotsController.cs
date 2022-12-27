using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/itemSlots")]
[ApiController]
public class ItemSlotsController
{
    private readonly IItemTemplateService _itemTemplateService;
    private readonly IMapper _mapper;

    public ItemSlotsController(IItemTemplateService itemTemplateService, IMapper mapper)
    {
        _itemTemplateService = itemTemplateService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ItemSlotResponse>>> GetAsync()
    {
        var skills = await _itemTemplateService.GetItemSlotsAsync();

        return _mapper.Map<List<ItemSlotResponse>>(skills);
    }
}