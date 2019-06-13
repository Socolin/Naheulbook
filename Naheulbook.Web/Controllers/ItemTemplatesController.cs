using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Filters;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/itemTemplates")]
    [ApiController]
    public class ItemTemplatesController : Controller
    {
        private readonly IItemTemplateService _itemTemplateService;
        private readonly IMapper _mapper;

        public ItemTemplatesController(IItemTemplateService itemTemplateService, IMapper mapper)
        {
            _itemTemplateService = itemTemplateService;
            _mapper = mapper;
        }

        [HttpGet("{itemTemplateId}")]
        public async Task<ActionResult<ItemTemplateResponse>> GetItemTemplateAsync(int itemTemplateId)
        {
            try
            {
                var itemTemplate = await _itemTemplateService.GetItemTemplateAsync(itemTemplateId);

                return _mapper.Map<ItemTemplateResponse>(itemTemplate);
            }
            catch (ItemTemplateNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPut("{itemTemplateId}")]
        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        public async Task<ActionResult<ItemTemplateResponse>> PutItemTemplateAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int itemTemplateId,
            ItemTemplateRequest request
        )
        {
            try
            {
                var itemTemplate = await _itemTemplateService.EditItemTemplateAsync(
                    executionContext,
                    itemTemplateId,
                    request
                );

                return _mapper.Map<ItemTemplateResponse>(itemTemplate);
            }
            catch (ItemTemplateNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        public async Task<JsonResult> PostCreateItemTemplateAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            ItemTemplateRequest request
        )
        {
            try
            {
                var itemTemplate = await _itemTemplateService.CreateItemTemplateAsync(executionContext, request);
                var itemTemplateResponse = _mapper.Map<ItemTemplateResponse>(itemTemplate);
                return new JsonResult(itemTemplateResponse)
                {
                    StatusCode = (int?) HttpStatusCode.Created
                };
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
        }
    }
}