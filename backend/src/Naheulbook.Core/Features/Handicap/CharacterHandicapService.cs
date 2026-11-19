using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Handicap;

public interface ICharacterHandicapService
{
    Task<HandicapEntity> AddHandicapAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        AddCharacterHandicapRequest request,
        CancellationToken cancellationToken = default
    );

    Task RemoveHandicapAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid handicapId,
        CancellationToken cancellationToken = default
    );
}

public class CharacterHandicapService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory,
    ICharacterHistoryUtil characterHistoryUtil
) : ICharacterHandicapService
{
    public async Task<HandicapEntity> AddHandicapAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        AddCharacterHandicapRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);

        var handicap = await uow.Handicaps.GetAsync(request.HandicapId, cancellationToken);
        if (handicap == null)
            throw new HandicapNotFoundException(request.HandicapId);

        var existing = await uow.CharacterHandicaps.GetByCharacterIdAndHandicapIdAsync(characterId, request.HandicapId, cancellationToken);
        if (existing != null)
            throw new HandicapAlreadyKnownException(request.HandicapId);

        uow.CharacterHandicaps.Add(new CharacterHandicapEntity
        {
            Character = character,
            Handicap = handicap,
        });

        character.AddHistoryEntry(characterHistoryUtil.CreateAddHandicap(characterId, handicap));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterAddHandicap(characterId, handicap);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();

        return handicap;
    }

    public async Task RemoveHandicapAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid handicapId,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);

        var characterHandicap = await uow.CharacterHandicaps.GetByCharacterIdAndHandicapIdAsync(characterId, handicapId, cancellationToken);
        if (characterHandicap == null)
            throw new CharacterHandicapNotFoundException(handicapId);

        uow.CharacterHandicaps.Remove(characterHandicap);
        character.AddHistoryEntry(characterHistoryUtil.CreateRemoveHandicap(characterId, characterHandicap.Handicap));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterRemoveHandicap(characterId, handicapId);

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
