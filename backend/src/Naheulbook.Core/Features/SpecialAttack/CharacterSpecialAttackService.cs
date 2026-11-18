using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.SpecialAttack;

public interface ICharacterSpecialAttackService
{
    Task<CharacterSpecialAttackEntity> AddSpecialAttackAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        AddCharacterSpecialAttackRequest request,
        CancellationToken cancellationToken = default
    );

    Task RemoveSpecialAttackAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid specialAttackId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Use a special attack: start its cooldown, and on success count it and give its XP to the character
    /// </summary>
    Task<UseSpecialAttackResult> UseSpecialAttackAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid specialAttackId,
        UseCharacterSpecialAttackRequest request,
        CancellationToken cancellationToken = default
    );

    Task<CharacterSpecialAttackEntity> UpdateSpecialAttackAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid specialAttackId,
        UpdateCharacterSpecialAttackRequest request,
        CancellationToken cancellationToken = default
    );
}

public class CharacterSpecialAttackService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory,
    ICharacterHistoryUtil characterHistoryUtil,
    IJsonUtil jsonUtil,
    ISpecialAttackCooldownUtil specialAttackCooldownUtil
) : ICharacterSpecialAttackService
{
    public async Task<CharacterSpecialAttackEntity> AddSpecialAttackAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        AddCharacterSpecialAttackRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);

        var specialAttack = await uow.SpecialAttacks.GetAsync(request.SpecialAttackId, cancellationToken);
        if (specialAttack == null)
            throw new SpecialAttackNotFoundException(request.SpecialAttackId);

        var existing = await uow.CharacterSpecialAttacks.GetByCharacterIdAndSpecialAttackIdAsync(characterId, request.SpecialAttackId, cancellationToken);
        if (existing != null)
            throw new SpecialAttackAlreadyKnownException(request.SpecialAttackId);

        var characterSpecialAttack = new CharacterSpecialAttackEntity
        {
            Character = character,
            SpecialAttack = specialAttack,
            Data = jsonUtil.SerializeNonNull(new CharacterSpecialAttackData()),
        };
        uow.CharacterSpecialAttacks.Add(characterSpecialAttack);

        character.AddHistoryEntry(characterHistoryUtil.CreateAddSpecialAttack(characterId, specialAttack));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterAddSpecialAttack(characterId, characterSpecialAttack);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();

        return characterSpecialAttack;
    }

    public async Task RemoveSpecialAttackAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid specialAttackId,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);
        var characterSpecialAttack = await GetCharacterSpecialAttackAsync(uow, characterId, specialAttackId, cancellationToken);

        uow.CharacterSpecialAttacks.Remove(characterSpecialAttack);
        character.AddHistoryEntry(characterHistoryUtil.CreateRemoveSpecialAttack(characterId, characterSpecialAttack.SpecialAttack));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterRemoveSpecialAttack(characterId, specialAttackId);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();
    }

    public async Task<UseSpecialAttackResult> UseSpecialAttackAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid specialAttackId,
        UseCharacterSpecialAttackRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);
        var characterSpecialAttack = await GetCharacterSpecialAttackAsync(uow, characterId, specialAttackId, cancellationToken);

        var specialAttackData = jsonUtil.DeserializeOrCreate<SpecialAttackData>(characterSpecialAttack.SpecialAttack.Data);
        var state = jsonUtil.DeserializeOrCreate<CharacterSpecialAttackData>(characterSpecialAttack.Data);

        if (!specialAttackCooldownUtil.IsAvailable(specialAttackData, state))
            throw new SpecialAttackNotAvailableException(specialAttackId);

        specialAttackCooldownUtil.RegisterUse(specialAttackData, state, request.Success);
        characterSpecialAttack.Data = jsonUtil.SerializeNonNull(state);

        character.AddHistoryEntry(characterHistoryUtil.CreateUseSpecialAttack(characterId, characterSpecialAttack.SpecialAttack, request.Success));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterUpdateSpecialAttack(characterId, characterSpecialAttack);

        if (request.Success && specialAttackData.Xp is > 0)
        {
            var xp = specialAttackData.Xp.Value;
            var oldExperience = character.Experience;
            character.Experience += xp;
            var info = $"+{xp} XP (coup spécial réussi : {characterSpecialAttack.SpecialAttack.Name})";
            character.AddHistoryEntry(characterHistoryUtil.CreateLogChangeExperience(character, oldExperience, character.Experience, info));
            notificationSession.NotifyCharacterChangeExperience(character);
        }

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();

        return new UseSpecialAttackResult(characterSpecialAttack, character.Experience);
    }

    public async Task<CharacterSpecialAttackEntity> UpdateSpecialAttackAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid specialAttackId,
        UpdateCharacterSpecialAttackRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);
        var characterSpecialAttack = await GetCharacterSpecialAttackAsync(uow, characterId, specialAttackId, cancellationToken);

        var state = jsonUtil.DeserializeOrCreate<CharacterSpecialAttackData>(characterSpecialAttack.Data);
        if (request.SuccessCount.HasValue)
            state.SuccessCount = Math.Max(0, request.SuccessCount.Value);
        if (request.ResetCooldowns)
            state.Cooldowns.Clear();
        characterSpecialAttack.Data = jsonUtil.SerializeNonNull(state);

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterUpdateSpecialAttack(characterId, characterSpecialAttack);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();

        return characterSpecialAttack;
    }

    private async Task<CharacterEntity> GetCharacterAsync(
        IUnitOfWork uow,
        NaheulbookExecutionContext executionContext,
        int characterId,
        CancellationToken cancellationToken
    )
    {
        var character = await uow.Characters.GetWithGroupAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        return character;
    }

    private static async Task<CharacterSpecialAttackEntity> GetCharacterSpecialAttackAsync(
        IUnitOfWork uow,
        int characterId,
        Guid specialAttackId,
        CancellationToken cancellationToken
    )
    {
        var characterSpecialAttack = await uow.CharacterSpecialAttacks.GetByCharacterIdAndSpecialAttackIdAsync(characterId, specialAttackId, cancellationToken);
        if (characterSpecialAttack == null)
            throw new CharacterSpecialAttackNotFoundException(specialAttackId);
        return characterSpecialAttack;
    }
}

public record UseSpecialAttackResult(CharacterSpecialAttackEntity CharacterSpecialAttack, int Experience);
