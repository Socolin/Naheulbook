using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Services
{
    public interface ICharacterService
    {
        Task<List<Character>> GetCharacterListAsync(NaheulbookExecutionContext executionContext);
        Task<Character> CreateCharacterAsync(NaheulbookExecutionContext executionContext, CreateCharacterRequest request);
        Task<Character> LoadCharacterDetailsAsync(NaheulbookExecutionContext executionContext, int characterId);
        Task<Item> AddItemToCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CreateItemRequest request);
        Task<List<Loot>> GetCharacterLootsAsync(NaheulbookExecutionContext executionContext, int characterId);
        Task<List<IHistoryEntry>> GetCharacterHistoryEntryAsync(NaheulbookExecutionContext executionContext, int characterId, int page);
        Task EnsureUserCanAccessCharacterAsync(NaheulbookExecutionContext executionContext, int characterId);
        Task UpdateCharacterStatAsync(NaheulbookExecutionContext executionContext, int characterId, PatchCharacterStatsRequest request);
        Task SetCharacterAdBonusStatAsync(NaheulbookExecutionContext executionContext, int characterId, PutStatBonusAdRequest request);
        Task<CharacterModifier> AddModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, AddCharacterModifierRequest request);
        Task DeleteModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId);
        Task<CharacterModifier> ToggleModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId);
    }

    public class CharacterService : ICharacterService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ICharacterFactory _characterFactory;
        private readonly IItemService _itemService;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly ICharacterHistoryUtil _characterHistoryUtil;
        private readonly IMapper _mapper;
        private readonly ICharacterModifierUtil _characterModifierUtil;
        private readonly IChangeNotifier _changeNotifier;

        public CharacterService(
            IUnitOfWorkFactory unitOfWorkFactory,
            ICharacterFactory characterFactory,
            IItemService itemService,
            IAuthorizationUtil authorizationUtil,
            ICharacterHistoryUtil characterHistoryUtil,
            IMapper mapper,
            ICharacterModifierUtil characterModifierUtil,
            IChangeNotifier changeNotifier
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _characterFactory = characterFactory;
            _itemService = itemService;
            _authorizationUtil = authorizationUtil;
            _characterHistoryUtil = characterHistoryUtil;
            _mapper = mapper;
            _characterModifierUtil = characterModifierUtil;
            _changeNotifier = changeNotifier;
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

            var item = await _itemService.AddItemToAsync(executionContext, ItemOwnerType.Character, characterId, request);

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                uow.CharacterHistoryEntries.Add(_characterHistoryUtil.CreateLogAddItem(characterId, item));
                await uow.CompleteAsync();
            }

            await _changeNotifier.NotifyCharacterAddItemAsync(characterId, item);

            return item;
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

        public async Task<List<IHistoryEntry>> GetCharacterHistoryEntryAsync(NaheulbookExecutionContext executionContext, int characterId, int page)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);

                return await uow.Characters.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, page, character.Group?.MasterId == characterId);
            }
        }

        public async Task EnsureUserCanAccessCharacterAsync(NaheulbookExecutionContext executionContext, int characterId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);
            }
        }

        public async Task UpdateCharacterStatAsync(NaheulbookExecutionContext executionContext, int characterId, PatchCharacterStatsRequest request)
        {
            var notificationTasks = new List<Task>();

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);

                if (request.Ev.HasValue)
                {
                    uow.CharacterHistoryEntries.Add(_characterHistoryUtil.CreateLogChangeEv(character, character.Ev, request.Ev));
                    character.Ev = request.Ev;
                    notificationTasks.Add(_changeNotifier.NotifyCharacterChangeEvAsync(character));
                }

                if (request.Ea.HasValue)
                {
                    uow.CharacterHistoryEntries.Add(_characterHistoryUtil.CreateLogChangeEa(character, character.Ea, request.Ea));
                    character.Ea = request.Ea;
                    notificationTasks.Add(_changeNotifier.NotifyCharacterChangeEaAsync(character));
                }

                if (request.FatePoint.HasValue)
                {
                    uow.CharacterHistoryEntries.Add(_characterHistoryUtil.CreateLogChangeFatePoint(character, character.FatePoint, request.FatePoint));
                    character.FatePoint = request.FatePoint.Value;
                    notificationTasks.Add(_changeNotifier.NotifyCharacterChangeFatePointAsync(character));
                }

                if (request.Experience.HasValue)
                {
                    uow.CharacterHistoryEntries.Add(_characterHistoryUtil.CreateLogChangeExperience(character, character.Experience, request.Experience));
                    character.Experience = request.Experience.Value;
                    notificationTasks.Add(_changeNotifier.NotifyCharacterChangeExperienceAsync(character));
                }

                if (request.Sex != null)
                {
                    uow.CharacterHistoryEntries.Add(_characterHistoryUtil.CreateLogChangeSex(character, character.Sex, request.Sex));
                    character.Sex = request.Sex;
                    notificationTasks.Add(_changeNotifier.NotifyCharacterChangeSexAsync(character));
                }

                if (request.Name != null)
                {
                    uow.CharacterHistoryEntries.Add(_characterHistoryUtil.CreateLogChangeName(character, character.Name, request.Name));
                    character.Name = request.Name;
                    notificationTasks.Add(_changeNotifier.NotifyCharacterChangeNameAsync(character));
                }

                await uow.CompleteAsync();
            }

            await Task.WhenAll(notificationTasks);
        }

        public async Task SetCharacterAdBonusStatAsync(NaheulbookExecutionContext executionContext, int characterId, PutStatBonusAdRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);

                character.StatBonusAd = request.Stat;

                await _changeNotifier.NotifyCharacterSetStatBonusAdAsync(character.Id, request.Stat);

                await uow.CompleteAsync();
            }
        }

        public async Task<CharacterModifier> AddModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, AddCharacterModifierRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);

                var characterModifier = _mapper.Map<CharacterModifier>(request);
                characterModifier.Character = character;

                uow.CharacterModifiers.Add(characterModifier);
                uow.CharacterHistoryEntries.Add(_characterHistoryUtil.CreateLogAddModifier(character, characterModifier));

                await uow.CompleteAsync();

                await _changeNotifier.NotifyCharacterAddModifierAsync(characterId, _mapper.Map<ActiveStatsModifier>(characterModifier));

                return characterModifier;
            }
        }

        public async Task DeleteModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);

                var characterModifier = await uow.CharacterModifiers.GetByIdAndCharacterIdAsync(characterId, characterModifierId);
                if (characterModifier == null)
                    throw new CharacterModifierNotFoundException(characterModifierId);

                uow.CharacterModifiers.Remove(characterModifier);
                uow.CharacterHistoryEntries.Add(_characterHistoryUtil.CreateLogRemoveModifier(characterId, characterModifierId));

                await uow.CompleteAsync();

                await _changeNotifier.NotifyCharacterRemoveModifierAsync(characterId, characterModifierId);
            }
        }

        public async Task<CharacterModifier> ToggleModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);

                var characterModifier = await uow.CharacterModifiers.GetByIdAndCharacterIdAsync(characterId, characterModifierId);
                if (characterModifier == null)
                    throw new CharacterModifierNotFoundException(characterModifierId);

                _characterModifierUtil.ToggleModifier(character, characterModifier);

                await uow.CompleteAsync();

                await _changeNotifier.NotifyUpdateCharacterModifierAsync(characterId, _mapper.Map<ActiveStatsModifier>(characterModifier));

                return characterModifier;
            }
        }
    }
}