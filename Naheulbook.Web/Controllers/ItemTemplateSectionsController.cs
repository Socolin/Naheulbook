using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
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

        [HttpGet]
        public async Task<ActionResult<List<ItemTemplateSectionResponse>>> GetItemTemplateSectionsAsync()
        {
            var sections = await _itemTemplateSectionService.GetAllSectionsAsync();
            return _mapper.Map<List<ItemTemplateSectionResponse>>(sections);
        }

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
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
            }
        }

        [HttpGet("{SectionId}")]
        public async Task<ActionResult<List<ItemTemplateResponse>>> GetItemTemplatesAsync(
            [FromServices] OptionalNaheulbookExecutionContext executionContext,
            int sectionId
        )
        {
            var sections = await _itemTemplateSectionService.GetItemTemplatesBySectionAsync(sectionId, executionContext.ExecutionExecutionContext?.UserId);
            return _mapper.Map<List<ItemTemplateResponse>>(sections);
        }
    }
}