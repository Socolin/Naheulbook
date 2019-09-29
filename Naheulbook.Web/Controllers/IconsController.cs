using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/icons")]
    [ApiController]
    public class IconsController : ControllerBase
    {
        private readonly IIconService _iconService;

        public IconsController(
            IIconService iconService
        )
        {
            _iconService = iconService;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> GetAllIconsAsync()
        {
            var icons = await _iconService.GetAllIconsAsync();
            return icons.Select(i => i.Name).ToList();
        }
    }
}