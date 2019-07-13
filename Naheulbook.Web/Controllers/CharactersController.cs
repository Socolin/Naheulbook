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
    public class CharactersController : Controller
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

        [HttpGet("{CharacterId:int:min(1)}")]
        public async Task<ActionResult<CharacterResponse>> GetCharacterDetailsAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId
        )
        {
            try
            {
                var character = await _characterService.LoadCharacterDetailsAsync(executionContext, characterId);

                if (executionContext.UserId == character.Group?.MasterId)
                    return _mapper.Map<CharacterFoGmResponse>(character);

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

        [HttpGet("search")]
        public async Task<ActionResult<List<CharacterSearchResponse>>> GetSearchCharacterAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromQuery] string filter
        )
        {
            var characters = await _characterService.SearchCharactersAsync(filter);
            return _mapper.Map<List<CharacterSearchResponse>>(characters);
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

        [HttpPost("{CharacterId:int:min(1)}/items")]
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

        [HttpPost("{CharacterId:int:min(1)}/modifiers")]
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

        [HttpDelete("{CharacterId:int:min(1)}/modifiers/{CharacterModifierId}")]
        public async Task<IActionResult> DeleteModifiersAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId,
            [FromRoute] int characterModifierId
        )
        {
            try
            {
                await _characterService.DeleteModifiersAsync(executionContext, characterId, characterModifierId);
                return NoContent();
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (CharacterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
            catch (CharacterModifierNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
        }

        [HttpPost("{CharacterId:int:min(1)}/modifiers/{CharacterModifierId}/toggle")]
        public async Task<ActionResult<ActiveStatsModifier>> PostToggleModifiersAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId,
            [FromRoute] int characterModifierId
        )
        {
            try
            {
                var characterModifier = await _characterService.ToggleModifiersAsync(executionContext, characterId, characterModifierId);
                return _mapper.Map<ActiveStatsModifier>(characterModifier);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HttpErrorException(HttpStatusCode.Forbidden, ex);
            }
            catch (CharacterNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
            catch (CharacterModifierNotFoundException ex)
            {
                throw new HttpErrorException(HttpStatusCode.NotFound, ex);
            }
            catch (CharacterModifierNotReusableException ex)
            {
                throw new HttpErrorException(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet("{CharacterId:int:min(1)}/loots")]
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

        [HttpGet("{CharacterId:int:min(1)}/history")]
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

        [HttpPatch("{CharacterId:int:min(1)}")]
        public async Task<IActionResult> PatchCharacterAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId,
            PatchCharacterRequest request
        )
        {
            try
            {
                await _characterService.UpdateCharacterAsync(executionContext, characterId, request);
                return NoContent();
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

        [HttpPut("{CharacterId:int:min(1)}/statBonusAd")]
        public async Task<IActionResult> PutStatBonusAdAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int characterId,
            PutStatBonusAdRequest request
        )
        {
            try
            {
                await _characterService.SetCharacterAdBonusStatAsync(executionContext, characterId, request);
                return NoContent();
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