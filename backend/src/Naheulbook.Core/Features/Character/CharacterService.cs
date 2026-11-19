using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Job;
using Naheulbook.Core.Features.Mastery;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Features.Talent;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Extensions;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterService
{
    Task<List<CharacterEntity>> GetCharacterListAsync(NaheulbookExecutionContext executionContext, CancellationToken cancellationToken = default);
    Task<CharacterEntity> CreateCharacterAsync(NaheulbookExecutionContext executionContext, CreateCharacterRequest request, CancellationToken cancellationToken = default);
    Task<CharacterEntity> CreateCustomCharacterAsync(NaheulbookExecutionContext executionContext, CreateCustomCharacterRequest request, CancellationToken cancellationToken = default);
    Task<CharacterEntity> LoadCharacterDetailsAsync(NaheulbookExecutionContext executionContext, int characterId, CancellationToken cancellationToken = default);
    Task<ItemEntity> AddItemToCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CreateItemRequest request, CancellationToken cancellationToken = default);
    Task<List<LootEntity>> GetCharacterLootsAsync(NaheulbookExecutionContext executionContext, int characterId, CancellationToken cancellationToken = default);
    Task<List<IHistoryEntry>> GetCharacterHistoryEntryAsync(NaheulbookExecutionContext executionContext, int characterId, int page, CancellationToken cancellationToken = default);
    Task<bool> EnsureUserCanAccessCharacterAndGetIfIsGroupMasterAsync(NaheulbookExecutionContext executionContext, int characterId, CancellationToken cancellationToken = default);
    Task UpdateCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, PatchCharacterRequest request, CancellationToken cancellationToken = default);
    Task SetCharacterAdBonusStatAsync(NaheulbookExecutionContext executionContext, int characterId, PutStatBonusAdRequest request, CancellationToken cancellationToken = default);
    Task<CharacterModifierEntity> AddModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, AddCharacterModifierRequest request, CancellationToken cancellationToken = default);
    Task DeleteModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId, CancellationToken cancellationToken = default);
    Task<CharacterModifierEntity> ToggleModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId, CancellationToken cancellationToken = default);
    Task<List<CharacterEntity>> SearchCharactersAsync(string filter, CancellationToken cancellationToken = default);
    Task<LevelUpResult> LevelUpCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterLevelUpRequest request, CancellationToken cancellationToken = default);
    Task AddJobAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterAddJobRequest request, CancellationToken cancellationToken = default);
    Task RemoveJobAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterRemoveJobRequest request, CancellationToken cancellationToken = default);
    Task QuitGroupAsync(NaheulbookExecutionContext executionContext, int characterId, CancellationToken cancellationToken = default);
}

public class CharacterService(
    IUnitOfWorkFactory unitOfWorkFactory,
    ICharacterFactory characterFactory,
    IItemService itemService,
    IAuthorizationUtil authorizationUtil,
    ICharacterHistoryUtil characterHistoryUtil,
    ICharacterModifierMapper characterModifierMapper,
    ICharacterModifierUtil characterModifierUtil,
    INotificationSessionFactory notificationSessionFactory,
    ICharacterUtil characterUtil,
    IItemUtil itemUtil
) : ICharacterService
{
    public async Task<List<CharacterEntity>> GetCharacterListAsync(NaheulbookExecutionContext executionContext, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Characters.GetForSummaryByOwnerIdAsync(executionContext.UserId, cancellationToken);
    }

    public async Task<CharacterEntity> CreateCharacterAsync(NaheulbookExecutionContext executionContext, CreateCharacterRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = characterFactory.CreateCharacter(request);

        if (request.GroupId.HasValue)
        {
            var group = await uow.Groups.GetAsync(request.GroupId.Value, cancellationToken);
            if (group == null)
                throw new GroupNotFoundException(request.GroupId.Value);
            authorizationUtil.EnsureIsGroupOwner(executionContext, group);
            character.Group = group;
        }

        character.OwnerId = executionContext.UserId;
        character.Items = await itemUtil.CreateInitialPlayerInventoryAsync(request.Money, cancellationToken);

        uow.Characters.Add(character);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return character;
    }

    public async Task<CharacterEntity> CreateCustomCharacterAsync(NaheulbookExecutionContext executionContext, CreateCustomCharacterRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = characterFactory.CreateCustomCharacter(request);

        if (request.GroupId.HasValue)
        {
            var group = await uow.Groups.GetAsync(request.GroupId.Value, cancellationToken);
            if (group == null)
                throw new GroupNotFoundException(request.GroupId.Value);
            authorizationUtil.EnsureIsGroupOwner(executionContext, @group);
            character.Group = @group;
        }

        character.OwnerId = executionContext.UserId;
        uow.Characters.Add(character);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return character;
    }

    public async Task<CharacterEntity> LoadCharacterDetailsAsync(NaheulbookExecutionContext executionContext, int characterId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithAllDataAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        return character;
    }

    public async Task<ItemEntity> AddItemToCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CreateItemRequest request, CancellationToken cancellationToken = default)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
            if (character == null)
                throw new CharacterNotFoundException(characterId);

            authorizationUtil.EnsureCharacterAccess(executionContext, character);
        }

        var item = await itemService.AddItemToAsync(ItemOwnerType.Character, characterId, request, cancellationToken);

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            uow.CharacterHistoryEntries.Add(characterHistoryUtil.CreateLogAddItem(characterId, item));
            await uow.SaveChangesAsync(DoNotInterrupt.Token);
        }

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterAddItem(characterId, item);
        await session.CommitAsync();

        return item;
    }

    public async Task<List<LootEntity>> GetCharacterLootsAsync(NaheulbookExecutionContext executionContext, int characterId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        if (!character.GroupId.HasValue)
            return [];

        return await uow.Loots.GetLootsVisibleByCharactersOfGroupAsync(character.GroupId.Value, cancellationToken);
    }

    public async Task<List<IHistoryEntry>> GetCharacterHistoryEntryAsync(NaheulbookExecutionContext executionContext, int characterId, int page, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        return await uow.Characters.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, page, character.Group?.MasterId == characterId, cancellationToken);
    }

    public async Task<bool> EnsureUserCanAccessCharacterAndGetIfIsGroupMasterAsync(NaheulbookExecutionContext executionContext, int characterId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        if (character.Group == null)
            return false;

        return authorizationUtil.IsGroupOwner(executionContext, character.Group);
    }

    public async Task UpdateCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, PatchCharacterRequest request, CancellationToken cancellationToken = default)
    {
        var notificationSession = notificationSessionFactory.CreateSession();

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
            if (character == null)
                throw new CharacterNotFoundException(characterId);

            authorizationUtil.EnsureCharacterAccess(executionContext, character);

            characterUtil.ApplyCharactersChange(executionContext, request, character, notificationSession);

            await uow.SaveChangesAsync(DoNotInterrupt.Token);
        }

        await notificationSession.CommitAsync();
    }

    public async Task SetCharacterAdBonusStatAsync(NaheulbookExecutionContext executionContext, int characterId, PutStatBonusAdRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        character.StatBonusAd = request.Stat;

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterSetStatBonusAd(characterId, request.Stat);
        await session.CommitAsync();
    }

    public async Task<CharacterModifierEntity> AddModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, AddCharacterModifierRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        var characterModifier = characterModifierMapper.ToEntity(request);
        characterModifier.Character = character;

        uow.CharacterModifiers.Add(characterModifier);
        uow.CharacterHistoryEntries.Add(characterHistoryUtil.CreateLogAddModifier(character, characterModifier));

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterAddModifier(characterId, characterModifier);
        await session.CommitAsync();

        return characterModifier;
    }

    public async Task DeleteModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        var characterModifier = await uow.CharacterModifiers.GetByIdAndCharacterIdAsync(characterId, characterModifierId, cancellationToken);
        if (characterModifier == null)
            throw new CharacterModifierNotFoundException(characterModifierId);

        uow.CharacterModifiers.Remove(characterModifier);
        uow.CharacterHistoryEntries.Add(characterHistoryUtil.CreateLogRemoveModifier(characterId, characterModifier));

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterRemoveModifier(characterId, characterModifierId);
        await session.CommitAsync();
    }

    public async Task<CharacterModifierEntity> ToggleModifiersAsync(NaheulbookExecutionContext executionContext, int characterId, int characterModifierId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        var characterModifier = await uow.CharacterModifiers.GetByIdAndCharacterIdAsync(characterId, characterModifierId, cancellationToken);
        if (characterModifier == null)
            throw new CharacterModifierNotFoundException(characterModifierId);

        characterModifierUtil.ToggleModifier(character, characterModifier);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyCharacterUpdateModifier(characterId, characterModifier);
        await session.CommitAsync();

        return characterModifier;
    }

    public async Task<List<CharacterEntity>> SearchCharactersAsync(string filter, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return [];

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Characters.SearchCharacterWithNoGroupByNameWithOriginWithOwner(filter, 10, cancellationToken);
    }

    public async Task<LevelUpResult> LevelUpCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterLevelUpRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        if (character.Level + 1 != request.TargetLevelUp)
            throw new InvalidTargetLevelUpRequestedException(character.Level, request.TargetLevelUp);

        var specialities = await uow.CharacterSpecialities.GetWithModiferWithSpecialByIdsAsync(request.SpecialityIds, cancellationToken);
        if (specialities.Count < request.SpecialityIds.Distinct().Count())
            throw new SpecialityNotFoundException();

        var talent = await GetLevelUpTalentAsync(uow, characterId, request, cancellationToken);
        var mastery = await GetLevelUpMasteryAsync(uow, characterId, request, cancellationToken);

        var origin = await uow.Origins.GetWithAllDataAsync(character.OriginId, cancellationToken);

        var levelUpResult = characterUtil.LevelUpCharacter(character, origin.NotNull(), specialities, request);

        uow.CharacterModifiers.AddRange(levelUpResult.NewModifiers);
        uow.CharacterSkills.AddRange(levelUpResult.NewSkills);
        uow.CharacterSpecialities.AddRange(levelUpResult.NewSpecialities);

        character.AddHistoryEntry(characterHistoryUtil.CreateLogLevelUp(character.Id, character.Level));
        if (talent != null)
        {
            uow.CharacterTalents.Add(new CharacterTalentEntity {Character = character, Talent = talent});
            levelUpResult.NewTalents.Add(talent);
            character.AddHistoryEntry(characterHistoryUtil.CreateAddTalent(character.Id, talent));
        }
        if (mastery != null)
        {
            uow.CharacterMasteries.Add(new CharacterMasteryEntity {Character = character, Mastery = mastery});
            levelUpResult.NewMasteries.Add(mastery);
            character.AddHistoryEntry(characterHistoryUtil.CreateAddMastery(character.Id, mastery));
        }
        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterLevelUp(character.Id, levelUpResult);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();

        return levelUpResult;
    }

    /// <summary>
    /// Talent chosen with the talent or mastery point of the level up, if any (only on levels giving such a point)
    /// </summary>
    private static async Task<TalentEntity?> GetLevelUpTalentAsync(IUnitOfWork uow, int characterId, CharacterLevelUpRequest request, CancellationToken cancellationToken)
    {
        EnsureValidTalentOrMasteryChoice(request);
        if (!request.TalentId.HasValue)
            return null;

        var talent = await uow.Talents.GetAsync(request.TalentId.Value, cancellationToken);
        if (talent == null)
            throw new TalentNotFoundException(request.TalentId.Value);
        if (await uow.CharacterTalents.GetByCharacterIdAndTalentIdAsync(characterId, talent.Id, cancellationToken) != null)
            throw new TalentAlreadyKnownException(talent.Id);
        return talent;
    }

    /// <summary>
    /// Mastery chosen with the talent or mastery point of the level up, if any (only on levels giving such a point)
    /// </summary>
    private static async Task<MasteryEntity?> GetLevelUpMasteryAsync(IUnitOfWork uow, int characterId, CharacterLevelUpRequest request, CancellationToken cancellationToken)
    {
        EnsureValidTalentOrMasteryChoice(request);
        if (!request.MasteryId.HasValue)
            return null;

        var mastery = await uow.Masteries.GetAsync(request.MasteryId.Value, cancellationToken);
        if (mastery == null)
            throw new MasteryNotFoundException(request.MasteryId.Value);
        if (await uow.CharacterMasteries.GetByCharacterIdAndMasteryIdAsync(characterId, mastery.Id, cancellationToken) != null)
            throw new MasteryAlreadyKnownException(mastery.Id);
        return mastery;
    }

    private static void EnsureValidTalentOrMasteryChoice(CharacterLevelUpRequest request)
    {
        if (!request.TalentId.HasValue && !request.MasteryId.HasValue)
            return;
        if (request.TalentId.HasValue && request.MasteryId.HasValue)
            throw new InvalidLevelUpTalentOrMasteryException("A level up gives either a talent or a mastery, not both");
        if (!TalentOrMasteryPointRule.GivesPointAtLevel(request.TargetLevelUp))
            throw new InvalidLevelUpTalentOrMasteryException($"Level {request.TargetLevelUp} does not give a talent or mastery point");
    }

    public async Task AddJobAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterAddJobRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupWithJobsWithOriginAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        if (character.Jobs.Any(x => x.JobId == request.JobId))
            throw new CharacterAlreadyKnowThisJobException(character.Id, request.JobId);

        var job = await uow.Jobs.GetAsync(request.JobId, cancellationToken);
        if (job == null)
            throw new JobNotFoundException(request.JobId);

        character.Jobs.Add(new CharacterJobEntity
            {
                Job = job,
            }
        );

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterAddJob(character.Id, job.Id);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();
    }

    public async Task RemoveJobAsync(NaheulbookExecutionContext executionContext, int characterId, CharacterRemoveJobRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupWithJobsWithOriginAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        var characterJob = character.Jobs.FirstOrDefault(x => x.JobId == request.JobId);
        if (characterJob == null)
            throw new CharacterDoNotKnowJobException(characterId, request.JobId);

        character.Jobs.Remove(characterJob);

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterRemoveJob(character.Id, request.JobId);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();
    }

    public async Task QuitGroupAsync(NaheulbookExecutionContext executionContext, int characterId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        character.GroupId = null;

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
    }
}