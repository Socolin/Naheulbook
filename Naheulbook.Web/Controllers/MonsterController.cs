using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers
{
    [ApiController]
    [Route("api/v2/monsters")]
    public class MonsterController : Controller
    {
        private readonly IMonsterService _monsterService;

        public MonsterController(IMonsterService monsterService)
        {
            _monsterService = monsterService;
        }

        [HttpPost("{MonsterId}/modifiers")]
        public async Task<CreatedActionResult<ActiveStatsModifier>> PostAddModifier(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int monsterId,
            ActiveStatsModifier statsModifier
        )
        {
            try
            {
                var modifier = await _monsterService.AddModifierAsync(executionContext, monsterId, statsModifier);
                return modifier;
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (MonsterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }
    }
}