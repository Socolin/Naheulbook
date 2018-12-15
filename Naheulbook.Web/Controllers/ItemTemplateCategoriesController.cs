using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Filters;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/itemTemplateCategories")]
    [ApiController]
    public class ItemTemplateCategoriesController : Controller
    {
        private readonly IItemTemplateCategoryService _itemTemplateCategoryService;
        private readonly IMapper _mapper;

        public ItemTemplateCategoriesController(IItemTemplateCategoryService itemTemplateCategoryService, IMapper mapper)
        {
            _itemTemplateCategoryService = itemTemplateCategoryService;
            _mapper = mapper;
        }

        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        [HttpPost]
        public async Task<JsonResult> PostCreateCategoryAsync(CreateItemTemplateCategoryRequest request)
        {
            try
            {
                var itemTemplateCategory = await _itemTemplateCategoryService.CreateItemTemplateCategoryAsync(HttpContext.GetExecutionContext(), request);
                var itemTemplateCategoryResponse = _mapper.Map<ItemTemplateCategoryResponse>(itemTemplateCategory);
                return new JsonResult(itemTemplateCategoryResponse)
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