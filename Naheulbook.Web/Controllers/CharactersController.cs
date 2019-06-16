using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Filters;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/characters")]
    [ApiController]
    public class CharactersController
    {
        private readonly ICharacterService _characterService;
        private readonly IMapper _mapper;

        public CharactersController(
            ICharacterService characterService,
            IMapper mapper
        )
        {
            _characterService = characterService;
            _mapper = mapper;
        }

        [HttpPost]
        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        public async Task<CreatedActionResult<CreateCharacterResponse>> PostCreateCharacterAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateCharacterRequest request
        )
        {
            var group = await _characterService.CreateCharacterAsync(executionContext, request);
            return _mapper.Map<CreateCharacterResponse>(group);
        }
    }
}