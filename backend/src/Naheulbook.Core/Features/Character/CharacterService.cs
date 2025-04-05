using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Job;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterService
{
    Task<List<CharacterEntity>> GetCharacterListAsync(NaheulbookExecutionContext executionContext);
    Task<CharacterEntity> CreateCharacterAsync(NaheulbookExecutionContext executionContext, CreateCharacterRequest request);
    Task<CharacterEntity> CreateCustomCharacterAsync(NaheulbookExecutionContext executionContext, CreateCustomCharacterRequest request);
    Task<CharacterEntity> LoadCharacterDetailsAsync(NaheulbookExecutionContext executionContext, int characterId);
    Task<ItemEntity> AddItemToCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CreateItemRequest request);
    Task<List<LootEntity>> GetCharacterLootsAsync(NaheulbookExecutionContext executionContext, int characterId);
    Task<List<IHistoryEntry>> GetCharacterHistoryEntryAsync(NaheulbookExecutionContext executionContext, int characterId, int page);
    Task<bool> EnsureUserCanAccessCharacterAndGetIfIsGroupMasterAsync(NaheulbookExecutionContext executionContext, int characterId);
    Task UpdateCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, PatchCharacterRequest request);
    Task SetCharacterAdBonusStatAsync(NaheulbookExecutionContext executionContext, int characterId, PutStatBonusAdRequest request);
    Task<CharacterModifierEntity> AddModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, AddCharacterModifierRequest request);
    Task DeleteModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId);
    Task<CharacterModifierEntity> ToggleModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId);
    Task<List<CharacterEntity>> SearchCharactersAsync(string filter);
    Task<LevelUpResult> LevelUpCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterLevelUpRequest request);
    Task AddJobAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterAddJobRequest request);
    Task RemoveJobAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterRemoveJobRequest request);
    Task QuitGroupAsync(NaheulbookExecutionContext executionContext, int characterId);
}

public class CharacterService(
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
) : ICharacterService
{
    public async Task<List<CharacterEntity>> GetCharacterListAsync(NaheulbookExecutionContext executionContext)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Characters.GetForSummaryByOwnerIdAsync(executionContext.UserId);
    }

    public async Task<CharacterEntity> CreateCharacterAsync(NaheulbookExecutionContext executionContext, CreateCharacterRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = characterFactory.CreateCharacter(request);

        if (request.GroupId.HasValue)
        {
            var group = await uow.Groups.GetAsync(request.GroupId.Value);
            if (group == null)
                throw new GroupNotFoundException(request.GroupId.Value);
            authorizationUtil.EnsureIsGroupOwner(executionContext, group);
            character.Group = group;
        }

        character.OwnerId = executionContext.UserId;
        character.Items = await itemUtil.CreateInitialPlayerInventoryAsync(request.Money);

        uow.Characters.Add(character);

        await uow.SaveChangesAsync();

        return character;
    }

    public async Task<CharacterEntity> CreateCustomCharacterAsync(NaheulbookExecutionContext executionContext, CreateCustomCharacterRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = characterFactory.CreateCustomCharacter(request);

        if (request.GroupId.HasValue)
        {
            var group = await uow.Groups.GetAsync(request.GroupId.Value);
            if (group == null)
                throw new GroupNotFoundException(request.GroupId.Value);
            authorizationUtil.EnsureIsGroupOwner(executionContext, @group);
            character.Group = @group;
        }

        character.OwnerId = executionContext.UserId;
        uow.Characters.Add(character);

        await uow.SaveChangesAsync();

        return character;
    }

    public async Task<CharacterEntity> LoadCharacterDetailsAsync(NaheulbookExecutionContext executionContext, int characterId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithAllDataAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        return character;
    }

    public async Task<ItemEntity> AddItemToCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CreateItemRequest request)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var character = await uow.Characters.GetWithGroupAsync(characterId);
            if (character == null)
                throw new CharacterNotFoundException(characterId);

            authorizationUtil.EnsureCharacterAccess(executionContext, character);
        }

        var item = await itemService.AddItemToAsync(ItemOwnerType.Character, characterId, request);

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            uow.CharacterHistoryEntries.Add(characterHistoryUtil.CreateLogAddItem(characterId, item));
            await uow.SaveChangesAsync();
        }

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterAddItem(characterId, item);
        await session.CommitAsync();

        return item;
    }

    public async Task<List<LootEntity>> GetCharacterLootsAsync(NaheulbookExecutionContext executionContext, int characterId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        if (!character.GroupId.HasValue)
            return [];

        return await uow.Loots.GetLootsVisibleByCharactersOfGroupAsync(character.GroupId.Value);
    }

    public async Task<List<IHistoryEntry>> GetCharacterHistoryEntryAsync(NaheulbookExecutionContext executionContext, int characterId, int page)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        return await uow.Characters.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, page, character.Group?.MasterId == characterId);
    }

    public async Task<bool> EnsureUserCanAccessCharacterAndGetIfIsGroupMasterAsync(NaheulbookExecutionContext executionContext, int characterId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        if (character.Group == null)
            return false;

        return authorizationUtil.IsGroupOwner(executionContext, character.Group);
    }

    public async Task UpdateCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, PatchCharacterRequest request)
    {
        var notificationSession = notificationSessionFactory.CreateSession();

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var character = await uow.Characters.GetWithGroupAsync(characterId);
            if (character == null)
                throw new CharacterNotFoundException(characterId);

            authorizationUtil.EnsureCharacterAccess(executionContext, character);

            characterUtil.ApplyCharactersChange(executionContext, request, character, notificationSession);

            await uow.SaveChangesAsync();
        }

        await notificationSession.CommitAsync();
    }

    public async Task SetCharacterAdBonusStatAsync(NaheulbookExecutionContext executionContext, int characterId, PutStatBonusAdRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        character.StatBonusAd = request.Stat;

        await uow.SaveChangesAsync();

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterSetStatBonusAd(characterId, request.Stat);
        await session.CommitAsync();
    }

    public async Task<CharacterModifierEntity> AddModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, AddCharacterModifierRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        var characterModifier = mapper.Map<CharacterModifierEntity>(request);
        characterModifier.Character = character;

        uow.CharacterModifiers.Add(characterModifier);
        uow.CharacterHistoryEntries.Add(characterHistoryUtil.CreateLogAddModifier(character, characterModifier));

        await uow.SaveChangesAsync();

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterAddModifier(characterId, characterModifier);
        await session.CommitAsync();

        return characterModifier;
    }

    public async Task DeleteModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        var characterModifier = await uow.CharacterModifiers.GetByIdAndCharacterIdAsync(characterId, characterModifierId);
        if (characterModifier == null)
            throw new CharacterModifierNotFoundException(characterModifierId);

        // TODO: workaround, will change after character history rework
        characterModifier.CharacterId = null;
        // uow.CharacterModifiers.Remove(characterModifier);
        uow.CharacterHistoryEntries.Add(characterHistoryUtil.CreateLogRemoveModifier(characterId, characterModifierId));

        await uow.SaveChangesAsync();

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterRemoveModifier(characterId, characterModifierId);
        await session.CommitAsync();
    }

    public async Task<CharacterModifierEntity> ToggleModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        var characterModifier = await uow.CharacterModifiers.GetByIdAndCharacterIdAsync(characterId, characterModifierId);
        if (characterModifier == null)
            throw new CharacterModifierNotFoundException(characterModifierId);

        characterModifierUtil.ToggleModifier(character, characterModifier);

        await uow.SaveChangesAsync();

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterUpdateModifier(characterId, characterModifier);
        await session.CommitAsync();

        return characterModifier;
    }

    public async Task<List<CharacterEntity>> SearchCharactersAsync(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return [];

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Characters.SearchCharacterWithNoGroupByNameWithOriginWithOwner(filter, 10);
    }

    public async Task<LevelUpResult> LevelUpCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterLevelUpRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        if (character.Level + 1 != request.TargetLevelUp)
            throw new InvalidTargetLevelUpRequestedException(character.Level, request.TargetLevelUp);

        var specialities = await uow.CharacterSpecialities.GetWithModiferWithSpecialByIdsAsync(request.SpecialityIds);
        if (specialities.Count < request.SpecialityIds.Distinct().Count())
            throw new SpecialityNotFoundException();

        var origin = await uow.Origins.GetWithAllDataAsync(character.OriginId);

        var levelUpResult = characterUtil.LevelUpCharacter(character, origin.NotNull(), specialities, request);

        uow.CharacterModifiers.AddRange(levelUpResult.NewModifiers);
        uow.CharacterSkills.AddRange(levelUpResult.NewSkills);
        uow.CharacterSpecialities.AddRange(levelUpResult.NewSpecialities);

        character.AddHistoryEntry(characterHistoryUtil.CreateLogLevelUp(character.Id, character.Level));
        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterLevelUp(character.Id, levelUpResult);

        await uow.SaveChangesAsync();
        await notificationSession.CommitAsync();

        return levelUpResult;
    }

    public async Task AddJobAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterAddJobRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupWithJobsWithOriginAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        if (character.Jobs.Any(x => x.JobId == request.JobId))
            throw new CharacterAlreadyKnowThisJobException(character.Id, request.JobId);

        var job = await uow.Jobs.GetAsync(request.JobId);
        if (job == null)
            throw new JobNotFoundException(request.JobId);

        character.Jobs.Add(new CharacterJobEntity
            {
                Job = job,
            }
        );

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterAddJob(character.Id, job.Id);

        await uow.SaveChangesAsync();
        await notificationSession.CommitAsync();
    }

    public async Task RemoveJobAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterRemoveJobRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupWithJobsWithOriginAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        var characterJob = character.Jobs.FirstOrDefault(x => x.JobId == request.JobId);
        if (characterJob == null)
            throw new CharacterDoNotKnowJobException(characterId, request.JobId);

        character.Jobs.Remove(characterJob);

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterRemoveJob(character.Id, request.JobId);

        await uow.SaveChangesAsync();
        await notificationSession.CommitAsync();
    }

    public async Task QuitGroupAsync(NaheulbookExecutionContext executionContext, int characterId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        character.GroupId = null;

        await uow.SaveChangesAsync();
    }
}