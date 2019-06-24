using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;
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

        [HttpGet]
        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        public async Task<ActionResult<List<CharacterSummaryResponse>>> GetCharactersListAsync(
            [FromServices] NaheulbookExecutionContext executionContext
        )
        {
            var characters = await _characterService.GetCharacterListAsync(executionContext);
            return _mapper.Map<List<CharacterSummaryResponse>>(characters);
        }

        [HttpGet("{CharacterId}")]
        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        public async Task<ActionResult<CharacterResponse>> GetCharacterDetailsAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId
        )
        {
            try
            {
                var characters = await _characterService.LoadCharacterDetailsAsync(executionContext, characterId);
                return _mapper.Map<CharacterResponse>(characters);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (CharacterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
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


        [HttpPost("{CharacterId}/items")]
        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        public async Task<CreatedActionResult<ItemResponse>> PostAddItemToCharacterInventory(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId,
            CreateItemRequest request
        )
        {
            try
            {
                var item = await _characterService.AddItemToCharacterAsync(executionContext, characterId, request);
                return _mapper.Map<ItemResponse>(item);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (CharacterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
            catch (ItemTemplateNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet("{CharacterId}/loots")]
        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        public async Task<ActionResult<List<LootResponse>>> GetCharacterLoots(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId
        )
        {
            try
            {
                var loots = await _characterService.GetCharacterLootsAsync(executionContext, characterId);
                return _mapper.Map<List<LootResponse>>(loots);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (CharacterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpGet("{CharacterId}/history")]
        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        public async Task<ActionResult<List<HistoryEntryResponse>>> GetCharacterHistoryEntryAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId,
            [FromQuery] int page
        )
        {
            try
            {
                var loots = await _characterService.GetCharacterHistoryEntryAsync(executionContext, characterId, page);
                return _mapper.Map<List<HistoryEntryResponse>>(loots);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (CharacterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }
    }
}