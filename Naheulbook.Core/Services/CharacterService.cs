using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface ICharacterService
    {
        Task<List<Character>> GetCharacterListAsync(NaheulbookExecutionContext executionContext);
        Task<Character> CreateCharacterAsync(NaheulbookExecutionContext executionContext, CreateCharacterRequest request);
        Task<Character> LoadCharacterDetailsAsync(NaheulbookExecutionContext executionContext, int characterId);
        Task<Item> AddItemToCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CreateItemRequest request);
        Task<List<Loot>> GetCharacterLootsAsync(NaheulbookExecutionContext executionContext, int characterId);
    }

    public class CharacterService : ICharacterService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ICharacterFactory _characterFactory;
        private readonly IItemService _itemService;
        private readonly IAuthorizationUtil _authorizationUtil;

        public CharacterService(
            IUnitOfWorkFactory unitOfWorkFactory,
            ICharacterFactory characterFactory,
            IItemService itemService,
            IAuthorizationUtil authorizationUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _characterFactory = characterFactory;
            _itemService = itemService;
            _authorizationUtil = authorizationUtil;
        }

        public async Task<List<Character>> GetCharacterListAsync(NaheulbookExecutionContext executionContext)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Characters.GetForSummaryByOwnerIdAsync(executionContext.UserId);
            }
        }

        public async Task<Character> CreateCharacterAsync(NaheulbookExecutionContext executionContext, CreateCharacterRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = _characterFactory.CreateCharacter(request);

                character.OwnerId = executionContext.UserId;

                uow.Characters.Add(character);
                await uow.CompleteAsync();

                return character;
            }
        }

        public async Task<Character> LoadCharacterDetailsAsync(NaheulbookExecutionContext executionContext, int characterId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithAllDataAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);

                return character;
            }
        }

        public async Task<Item> AddItemToCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CreateItemRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);
            }

            return await _itemService.AddItemToAsync(executionContext, ItemOwnerType.Character, characterId, request);
        }

        public async Task<List<Loot>> GetCharacterLootsAsync(NaheulbookExecutionContext executionContext, int characterId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);

                if (!character.GroupId.HasValue)
                    return new List<Loot>();

                return await uow.Loots.GetLootsVisibleByCharactersOfGroupAsync(character.GroupId.Value);
            }
        }
    }
}