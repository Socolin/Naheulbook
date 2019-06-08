using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Extensions;
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

        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        [HttpPost]
        public async Task<JsonResult> PostCreateItemTemplateAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateItemTemplateRequest request
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