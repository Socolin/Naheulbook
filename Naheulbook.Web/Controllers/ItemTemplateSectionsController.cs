using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
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
        public async Task<JsonResult> PostCreateSectionAsync(CreateItemTemplateSectionRequest request)
        {
            var itemTemplateSection = await _itemTemplateSectionService.CreateItemTemplateSectionAsync(HttpContext.GetExecutionContext(), request);
            var itemTemplateSectionResponse = _mapper.Map<ItemTemplateSectionResponse>(itemTemplateSection);
            return new JsonResult(itemTemplateSectionResponse)
            {
                StatusCode = (int?) HttpStatusCode.Created
            };
        }
    }
}