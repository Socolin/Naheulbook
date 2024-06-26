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

namespace Naheulbook.Web.Controllers;

[ApiController]
[Route("/api/v2/npcs")]
public class NpcController(
    INpcService npcService,
    IMapper mapper
) : ControllerBase
{
    [HttpPut("{NpcId:int:min(1)}")]
    public async Task<ActionResult<NpcResponse>> PutEditNpcAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int npcId,
        NpcRequest request
    )
    {
        try
        {
            var npc = await npcService.EditNpcAsync(executionContext, npcId, request);
            return mapper.Map<NpcResponse>(npc);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (NpcNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
    }
}