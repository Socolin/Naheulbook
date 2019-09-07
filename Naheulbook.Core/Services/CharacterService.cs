using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
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
        Task<List<IHistoryEntry>> GetCharacterHistoryEntryAsync(NaheulbookExecutionContext executionContext, int characterId, int page);
        Task<bool> EnsureUserCanAccessCharacterAndGetIfIsGroupMasterAsync(NaheulbookExecutionContext executionContext, int characterId);
        Task UpdateCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, PatchCharacterRequest request);
        Task SetCharacterAdBonusStatAsync(NaheulbookExecutionContext executionContext, int characterId, PutStatBonusAdRequest request);
        Task<CharacterModifier> AddModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, AddCharacterModifierRequest request);
        Task DeleteModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId);
        Task<CharacterModifier> ToggleModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId);
        Task<List<Character>> SearchCharactersAsync(string filter);
        Task<LevelUpResult> LevelUpCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterLevelUpRequest request);
    }

    public class CharacterService : ICharacterService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ICharacterFactory _characterFactory;
        private readonly IItemService _itemService;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly ICharacterHistoryUtil _characterHistoryUtil;
        private readonly ICharacterUtil _characterUtil;
        private readonly IMapper _mapper;
        private readonly ICharacterModifierUtil _characterModifierUtil;
        private readonly INotificationSessionFactory _notificationSessionFactory;
        private readonly IItemUtil _itemUtil;

        public CharacterService(
            IUnitOfWorkFactory unitOfWorkFactory,
            ICharacterFactory characterFactory,
            IItemService itemService,
            IAuthorizationUtil authorizationUtil,
            ICharacterHistoryUtil characterHistoryUtil,
            IMapper mapper,
            ICharacterModifierUtil characterModifierUtil,
            INotificationSessionFactory notificationSessionFactory,
            ICharacterUtil characterUtil,
            IItemUtil itemUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _characterFactory = characterFactory;
            _itemService = itemService;
            _authorizationUtil = authorizationUtil;
            _characterHistoryUtil = characterHistoryUtil;
            _mapper = mapper;
            _characterModifierUtil = characterModifierUtil;
            _notificationSessionFactory = notificationSessionFactory;
            _characterUtil = characterUtil;
            _itemUtil = itemUtil;
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

                if (request.GroupId.HasValue)
                {
                    var group = await uow.Groups.GetAsync(request.GroupId.Value);
                    if (group == null)
                        throw new GroupNotFoundException(request.GroupId.Value);
                    _authorizationUtil.EnsureIsGroupOwner(executionContext, group);
                    character.Group = group;
                }

                character.OwnerId = executionContext.UserId;
                character.Items = await _itemUtil.CreateInitialPlayerInventoryAsync(request.Money);

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

            var item = await _itemService.AddItemToAsync(ItemOwnerType.Character, characterId, request);

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                uow.CharacterHistoryEntries.Add(_characterHistoryUtil.CreateLogAddItem(characterId, item));
                await uow.CompleteAsync();
            }

            var session = _notificationSessionFactory.CreateSession();
            session.NotifyCharacterAddItem(characterId, item);
            await session.CommitAsync();

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

        public async Task<bool> EnsureUserCanAccessCharacterAndGetIfIsGroupMasterAsync(NaheulbookExecutionContext executionContext, int characterId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);

                if (character.Group == null)
                    return false;

                return _authorizationUtil.IsGroupOwner(executionContext, character.Group);
            }
        }

        public async Task UpdateCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, PatchCharacterRequest request)
        {
            var notificationSession = _notificationSessionFactory.CreateSession();

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);

                _characterUtil.ApplyCharactersChange(executionContext, request, character, notificationSession);

                await uow.CompleteAsync();
            }

            await notificationSession.CommitAsync();
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

                await uow.CompleteAsync();

                var session = _notificationSessionFactory.CreateSession();
                session.NotifyCharacterSetStatBonusAd(characterId, request.Stat);
                await session.CommitAsync();
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

                var session = _notificationSessionFactory.CreateSession();
                session.NotifyCharacterAddModifier(characterId, characterModifier);
                await session.CommitAsync();

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

                // TODO: workaround, will change after character history rework
                characterModifier.CharacterId = null;
                // uow.CharacterModifiers.Remove(characterModifier);
                uow.CharacterHistoryEntries.Add(_characterHistoryUtil.CreateLogRemoveModifier(characterId, characterModifierId));

                await uow.CompleteAsync();

                var session = _notificationSessionFactory.CreateSession();
                session.NotifyCharacterRemoveModifier(characterId, characterModifierId);
                await session.CommitAsync();
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

                var session = _notificationSessionFactory.CreateSession();
                session.NotifyCharacterUpdateModifier(characterId, characterModifier);
                await session.CommitAsync();

                return characterModifier;
            }
        }

        public async Task<List<Character>> SearchCharactersAsync(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return new List<Character>();

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Characters.SearchCharacterWithNoGroupByNameWithOriginWithOwner(filter, 10);
            }
        }

        public async Task<LevelUpResult> LevelUpCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterLevelUpRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = await uow.Characters.GetWithGroupAsync(characterId);
                if (character == null)
                    throw new CharacterNotFoundException(characterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);

                if (character.Level + 1 != request.TargetLevelUp)
                    throw new InvalidTargetLevelUpRequestedException(character.Level, request.TargetLevelUp);

                var specialities = await uow.CharacterSpecialities.GetWithModiferWithSpecialByIdsAsync(request.SpecialityIds);
                if (specialities.Count < request.SpecialityIds.Distinct().Count())
                    throw new SpecialityNotFoundException();

                var origin = await uow.Origins.GetWithAllDataAsync(character.OriginId);

                var levelUpResult = _characterUtil.LevelUpCharacter(character, origin, specialities, request);

                uow.CharacterModifiers.AddRange(levelUpResult.NewModifiers);
                uow.CharacterSkills.AddRange(levelUpResult.NewSkills);
                uow.CharacterSpecialities.AddRange(levelUpResult.NewSpecialities);

                character.AddHistoryEntry(_characterHistoryUtil.CreateLogLevelUp(character.Id, character.Level));
                var notificationSession = _notificationSessionFactory.CreateSession();
                notificationSession.NotifyCharacterLevelUp(character.Id, levelUpResult);

                await uow.CompleteAsync();
                await notificationSession.CommitAsync();

                return levelUpResult;
            }
        }
    }
}