using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Item;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/itemSlots")]
[ApiController]
public class ItemSlotsController(IItemTemplateService itemTemplateService, IItemTemplateMapper itemTemplateMapper)
{
    [HttpGet]
    public async Task<ActionResult<List<ItemSlotResponse>>> GetAsync(CancellationToken cancellationToken)
    {
        var skills = await itemTemplateService.GetItemSlotsAsync(cancellationToken);

        return skills.Select(itemTemplateMapper.ToResponse).ToList();
    }
}