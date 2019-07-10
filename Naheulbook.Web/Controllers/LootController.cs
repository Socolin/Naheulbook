using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;

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
            try
            {
                await _lootService.UpdateLootVisibilityAsync(executionContext, lootId, request);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (LootNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpDelete("{LootId:int:min(1)}")]
        public async Task<IActionResult> DeleteLootAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int lootId
        )
        {
            try
            {
                await _lootService.DeleteLootAsync(executionContext, lootId);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (LootNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }
    }
}