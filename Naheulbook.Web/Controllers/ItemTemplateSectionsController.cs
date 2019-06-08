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
    [Route("api/v2/itemTemplateSections")]
    [ApiController]
    public class ItemTemplateSectionsController : Controller
    {
        private readonly IItemTemplateSectionService _itemTemplateSectionService;
        private readonly IMapper _mapper;

        public ItemTemplateSectionsController(IItemTemplateSectionService itemTemplateSectionService, IMapper mapper)
        {
            _itemTemplateSectionService = itemTemplateSectionService;
            _mapper = mapper;
        }

        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        [HttpPost]
        public async Task<JsonResult> PostCreateSectionAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateItemTemplateSectionRequest request
        )
        {
            try
            {
                var itemTemplateSection = await _itemTemplateSectionService.CreateItemTemplateSectionAsync(executionContext, request);
                var itemTemplateSectionResponse = _mapper.Map<ItemTemplateSectionResponse>(itemTemplateSection);
                return new JsonResult(itemTemplateSectionResponse)
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