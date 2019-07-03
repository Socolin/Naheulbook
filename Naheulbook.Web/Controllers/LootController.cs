using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/loots")]
    [ApiController]
    public class LootController : Controller
    {
        private readonly ILootService _lootService;

        public LootController(ILootService lootService)
        {
            _lootService = lootService;
        }

        [HttpPut("{LootId:int:min(1)}/visibility")]
        public async Task<IActionResult> PutLootVisibilityAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int lootId,
            PutLootVisibilityRequest request
        )
        {
            await _lootService.UpdateLootVisibilityAsync(executionContext, lootId, request);
            return NoContent();
        }
    }
}