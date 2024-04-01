using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/itemSlots")]
[ApiController]
public class ItemSlotsController(IItemTemplateService itemTemplateService, IMapper mapper)
{
    [HttpGet]
    public async Task<ActionResult<List<ItemSlotResponse>>> GetAsync()
    {
        var skills = await itemTemplateService.GetItemSlotsAsync();

        return mapper.Map<List<ItemSlotResponse>>(skills);
    }
}