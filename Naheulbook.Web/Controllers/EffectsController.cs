using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/effectCategories")]
    [ApiController]
    public class EffectsController : Controller
    {
        private readonly IEffectService _effectService;
        private readonly IMapper _mapper;

        public EffectsController(IEffectService effectService, IMapper mapper)
        {
            _effectService = effectService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<EffectTypeResponse>>> GetEffectCategoriesAsync()
        {
            var effectCategories = await _effectService.GetEffectCategoriesAsync();
            return _mapper.Map<List<EffectTypeResponse>>(effectCategories);
        }

        [HttpGet("{categoryId:int:min(1)}/effects")]
        public async Task<ActionResult<List<EffectResponse>>> GetEffectsAsync(long categoryId)
        {
            var effects = await _effectService.GetEffectsByCategoryAsync(categoryId);
            return _mapper.Map<List<EffectResponse>>(effects);
        }
    }
}