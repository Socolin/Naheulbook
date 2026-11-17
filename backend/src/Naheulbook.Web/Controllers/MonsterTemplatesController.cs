using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/monsterTemplates")]
[ApiController]
public class MonsterTemplatesController(IMonsterTemplateMapper monsterTemplateMapper, IMonsterTemplateService monsterTemplateService)
{
    [HttpGet]
    public async Task<ActionResult<List<MonsterTemplateResponse>>> GetMonsterListAsync(CancellationToken cancellationToken)
    {
        var monsters = await monsterTemplateService.GetAllMonstersAsync(cancellationToken);
        return monsters.Select(monsterTemplateMapper.ToResponse).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<MonsterTemplateResponse>> PostAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        MonsterTemplateRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var createdMonster = await monsterTemplateService.CreateMonsterTemplateAsync(executionContext, request, cancellationToken);

            var result = monsterTemplateMapper.ToResponse(createdMonster);
            return new JsonResult(result) {StatusCode = StatusCodes.Status201Created};
        }
        catch (MonsterSubCategoryNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }

    [HttpPut("{MonsterTemplateId}")]
    public async Task<ActionResult<MonsterTemplateResponse>> PutAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterTemplateId,
        MonsterTemplateRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var monster = await monsterTemplateService.EditMonsterTemplateAsync(executionContext, monsterTemplateId, request, cancellationToken);

            var result = monsterTemplateMapper.ToResponse(monster);
            return new JsonResult(result);
        }
        catch (MonsterSubCategoryNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (MonsterTemplateNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<MonsterTemplateResponse>>> GetSearchMonsterTemplateAsync(
        [FromQuery] string filter,
        [FromQuery] int? monsterTypeId,
        [FromQuery] int? monsterSubCategoryId,
        CancellationToken cancellationToken
    )
    {
        var monsterTemplates = await monsterTemplateService.SearchMonsterAsync(filter, monsterTypeId, monsterSubCategoryId, cancellationToken);
        return monsterTemplates.Select(monsterTemplateMapper.ToResponse).ToList();
    }
}