using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Mastery;

public interface ICharacterMasteryService
{
    Task<MasteryEntity> AddMasteryAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        AddCharacterMasteryRequest request,
        CancellationToken cancellationToken = default
    );

    Task RemoveMasteryAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid masteryId,
        CancellationToken cancellationToken = default
    );
}

public class CharacterMasteryService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory,
    ICharacterHistoryUtil characterHistoryUtil
) : ICharacterMasteryService
{
    public async Task<MasteryEntity> AddMasteryAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        AddCharacterMasteryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);

        var mastery = await uow.Masteries.GetAsync(request.MasteryId, cancellationToken);
        if (mastery == null)
            throw new MasteryNotFoundException(request.MasteryId);

        var existing = await uow.CharacterMasteries.GetByCharacterIdAndMasteryIdAsync(characterId, request.MasteryId, cancellationToken);
        if (existing != null)
            throw new MasteryAlreadyKnownException(request.MasteryId);

        uow.CharacterMasteries.Add(new CharacterMasteryEntity
        {
            Character = character,
            Mastery = mastery,
        });

        character.AddHistoryEntry(characterHistoryUtil.CreateAddMastery(characterId, mastery));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterAddMastery(characterId, mastery);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();

        return mastery;
    }

    public async Task RemoveMasteryAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid masteryId,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);

        var characterMastery = await uow.CharacterMasteries.GetByCharacterIdAndMasteryIdAsync(characterId, masteryId, cancellationToken);
        if (characterMastery == null)
            throw new CharacterMasteryNotFoundException(masteryId);

        uow.CharacterMasteries.Remove(characterMastery);
        character.AddHistoryEntry(characterHistoryUtil.CreateRemoveMastery(characterId, characterMastery.Mastery));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterRemoveMastery(characterId, masteryId);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();
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
}
