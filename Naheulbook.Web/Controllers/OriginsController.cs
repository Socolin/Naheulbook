using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Services;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/origins")]
    [ApiController]
    public class OriginsController : ControllerBase
    {
        private readonly IOriginService _originService;
        private readonly IMapper _mapper;
        private readonly ICharacterRandomNameService _characterRandomNameService;

        public OriginsController(
            IOriginService originService,
            IMapper mapper,
            ICharacterRandomNameService characterRandomNameService
        )
        {
            _originService = originService;
            _mapper = mapper;
            _characterRandomNameService = characterRandomNameService;
        }

        [HttpGet]
        public async Task<ActionResult<List<OriginResponse>>> GetAsync()
        {
            var skills = await _originService.GetOriginsAsync();

            return _mapper.Map<List<OriginResponse>>(skills);
        }

        [HttpGet("{OriginId:int:min(1)}/randomCharacterName")]
        public async Task<RandomCharacterNameResponse> GetRandomCharacterNameAsync(
            [FromRoute] int originId,
            [FromQuery] string sex
        )
        {
            try
            {
                var randomName = await _characterRandomNameService.GenerateRandomCharacterNameAsync(originId, sex);
                return new RandomCharacterNameResponse
                {
                    Name = randomName
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
}