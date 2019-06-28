using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;
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
        public async Task<ActionResult<List<CharacterSummaryResponse>>> GetCharactersListAsync(
            [FromServices] NaheulbookExecutionContext executionContext
        )
        {
            var characters = await _characterService.GetCharacterListAsync(executionContext);
            return _mapper.Map<List<CharacterSummaryResponse>>(characters);
        }

        [HttpGet("{CharacterId}")]
        public async Task<ActionResult<CharacterResponse>> GetCharacterDetailsAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId
        )
        {
            try
            {
                var character = await _characterService.LoadCharacterDetailsAsync(executionContext, characterId);
                return _mapper.Map<CharacterResponse>(character);
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
        public async Task<CreatedActionResult<CreateCharacterResponse>> PostCreateCharacterAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateCharacterRequest request
        )
        {
            var group = await _characterService.CreateCharacterAsync(executionContext, request);
            return _mapper.Map<CreateCharacterResponse>(group);
        }


        [HttpPost("{CharacterId}/items")]
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

        [HttpPost("{CharacterId}/modifiers")]
        public async Task<CreatedActionResult<ActiveStatsModifier>> PostAddModifiersAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId,
            AddCharacterModifierRequest request
        )
        {
            try
            {
                var item = await _characterService.AddModifiersAsync(executionContext, characterId, request);
                return _mapper.Map<ActiveStatsModifier>(item);
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

        [HttpGet("{CharacterId}/loots")]
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

        [HttpPatch("{CharacterId}/stats")]
        public async Task<IActionResult> PatchCharacterStatsAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId,
            PatchCharacterStatsRequest request
        )
        {
            try
            {
                await _characterService.UpdateCharacterStatAsync(executionContext, characterId, request);
                return new NoContentResult();
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

        [HttpPut("{CharacterId}/statBonusAd")]
        public async Task<IActionResult> PutStatBonusAdAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId,
            PutStatBonusAdRequest request
        )
        {
            try
            {
                await _characterService.SetCharacterAdBonusStatAsync(executionContext, characterId, request);
                return new NoContentResult();
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