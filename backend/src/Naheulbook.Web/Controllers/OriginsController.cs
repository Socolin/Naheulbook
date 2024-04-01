using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Services;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/origins")]
[ApiController]
public class OriginsController(
    IOriginService originService,
    IMapper mapper,
    ICharacterRandomNameService characterRandomNameService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OriginResponse>>> GetAsync()
    {
        var skills = await originService.GetOriginsAsync();

        return mapper.Map<List<OriginResponse>>(skills);
    }

    [HttpGet("{OriginId:guid}/randomCharacterName")]
    public async Task<RandomCharacterNameResponse> GetRandomCharacterNameAsync(
        [FromRoute] Guid originId,
        [FromQuery] string sex
    )
    {
        try
        {
            var randomName = await characterRandomNameService.GenerateRandomCharacterNameAsync(originId, sex);
            return new RandomCharacterNameResponse
            {
                Name = randomName,
            };
        }
        catch (OriginNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (RandomNameGeneratorNotFound ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }
}