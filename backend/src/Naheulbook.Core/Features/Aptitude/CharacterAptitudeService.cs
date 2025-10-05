using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Features.Aptitude;

public interface ICharacterAptitudeService
{
    Task<CharacterAptitudeEntity> AddAptitudeAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        CharacterAddAptitudeRequest request,
        CancellationToken cancellationToken = default
    );

    Task RemoveAptitudeAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid aptitudeId,
        CancellationToken cancellationToken = default
    );

    Task UpdateCharacterAptitudeAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid aptitudeId,
        UpdateCharacterAptitudeRequest request,
        CancellationToken cancellationToken = default
    );
}

public class CharacterAptitudeService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory,
    ICharacterHistoryUtil characterHistoryUtil
) : ICharacterAptitudeService
{
    public async Task<CharacterAptitudeEntity> AddAptitudeAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        CharacterAddAptitudeRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupWithOriginAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        var aptitude = await uow.AptitudeRepository.GetAsync(request.AptitudeId);
        if (aptitude == null)
            throw new AptitudeNotFoundException(request.AptitudeId);

        if (aptitude.AptitudeGroupId != character.Origin.AptitudeGroupId)
            throw new AptitudeNotAvailableForOriginException(request.AptitudeId);

        var notificationSession = notificationSessionFactory.CreateSession();

        var characterAptitude = await uow.CharacterAptitudes.GetByCharacterIdAndAptitudeIdAsync(characterId, request.AptitudeId, cancellationToken);
        if (characterAptitude != null)
            characterAptitude.Count++;
        else
        {
            characterAptitude = new CharacterAptitudeEntity
            {
                Character = character,
                Aptitude = aptitude,
                Count = 1,
                Active = false,
            };
            uow.CharacterAptitudes.Add(characterAptitude);
        }

        character.AddHistoryEntry(characterHistoryUtil.CreateAddAptitude(characterId, aptitude.Id));

        notificationSession.NotifyCharacterAddAptitude(characterId, characterAptitude);

        await uow.SaveChangesAsync();
        await notificationSession.CommitAsync();

        return characterAptitude;
    }

    public async Task RemoveAptitudeAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid aptitudeId,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupWithOriginAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        var characterAptitude = await uow.CharacterAptitudes.GetByCharacterIdAndAptitudeIdAsync(characterId, aptitudeId, cancellationToken);
        if (characterAptitude == null)
            throw new AptitudeNotFoundException(aptitudeId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);


        characterAptitude.Count--;
        if (characterAptitude.Count <= 0)
            uow.CharacterAptitudes.Remove(characterAptitude);
        character.AddHistoryEntry(characterHistoryUtil.CreateRemoveAptitude(characterId, aptitudeId));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterRemoveAptitude(characterId, characterAptitude);

        await uow.SaveChangesAsync();
        await notificationSession.CommitAsync();
    }

    public async Task UpdateCharacterAptitudeAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid aptitudeId,
        UpdateCharacterAptitudeRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithGroupWithOriginAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        var characterAptitude = await uow.CharacterAptitudes.GetByCharacterIdAndAptitudeIdAsync(characterId, aptitudeId, cancellationToken);
        if (characterAptitude == null)
            throw new AptitudeNotFoundException(aptitudeId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        characterAptitude.Active = request.Active;

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterUpdateAptitude(characterId, characterAptitude);

        await uow.SaveChangesAsync();
        await notificationSession.CommitAsync();
    }
}