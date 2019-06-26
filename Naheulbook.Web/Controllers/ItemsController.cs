using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/items")]
    [ApiController]
    public class ItemsController
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
            JObject itemData
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
    }
}