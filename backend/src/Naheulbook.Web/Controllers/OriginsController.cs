using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Origin;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/origins")]
[ApiController]
public class OriginsController(
    IOriginService originService,
    IOriginMapper originMapper,
    ICharacterRandomNameService characterRandomNameService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OriginResponse>>> GetAsync()
    {
        var skills = await originService.GetOriginsAsync();

        return skills.Select(originMapper.ToResponse).ToList();
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
        catch (RandomNameGeneratorNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }
}