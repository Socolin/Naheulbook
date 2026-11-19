using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Talent;

public interface ICharacterTalentService
{
    Task<TalentEntity> AddTalentAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        AddCharacterTalentRequest request,
        CancellationToken cancellationToken = default
    );

    Task RemoveTalentAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid talentId,
        CancellationToken cancellationToken = default
    );
}

public class CharacterTalentService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory,
    ICharacterHistoryUtil characterHistoryUtil
) : ICharacterTalentService
{
    public async Task<TalentEntity> AddTalentAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        AddCharacterTalentRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);

        var talent = await uow.Talents.GetAsync(request.TalentId, cancellationToken);
        if (talent == null)
            throw new TalentNotFoundException(request.TalentId);

        var existing = await uow.CharacterTalents.GetByCharacterIdAndTalentIdAsync(characterId, request.TalentId, cancellationToken);
        if (existing != null)
            throw new TalentAlreadyKnownException(request.TalentId);

        uow.CharacterTalents.Add(new CharacterTalentEntity
        {
            Character = character,
            Talent = talent,
        });

        character.AddHistoryEntry(characterHistoryUtil.CreateAddTalent(characterId, talent));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterAddTalent(characterId, talent);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();

        return talent;
    }

    public async Task RemoveTalentAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid talentId,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);

        var characterTalent = await uow.CharacterTalents.GetByCharacterIdAndTalentIdAsync(characterId, talentId, cancellationToken);
        if (characterTalent == null)
            throw new CharacterTalentNotFoundException(talentId);

        uow.CharacterTalents.Remove(characterTalent);
        character.AddHistoryEntry(characterHistoryUtil.CreateRemoveTalent(characterId, characterTalent.Talent));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterRemoveTalent(characterId, talentId);

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
