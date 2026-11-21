using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Spell;

/// <summary>
/// Spell books given to a character on top of the ones of its job and specialities (rare book found as loot,
/// "Mutant pyromane" bienfait giving the fire spells of level 1 and 2…).
/// </summary>
public interface ICharacterSpellBookService
{
    Task<CharacterSpellBookEntity> AddSpellBookAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        AddCharacterSpellBookRequest request,
        CancellationToken cancellationToken = default
    );

    Task<CharacterSpellBookEntity> UpdateSpellBookAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid spellBookId,
        UpdateCharacterSpellBookRequest request,
        CancellationToken cancellationToken = default
    );

    Task RemoveSpellBookAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid spellBookId,
        CancellationToken cancellationToken = default
    );
}

public class CharacterSpellBookService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory,
    ICharacterHistoryUtil characterHistoryUtil
) : ICharacterSpellBookService
{
    public async Task<CharacterSpellBookEntity> AddSpellBookAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        AddCharacterSpellBookRequest request,
        CancellationToken cancellationToken = default
    )
    {
        EnsureValidMaxLevel(request.MaxLevel);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);

        var spellBook = await uow.SpellBooks.GetAsync(request.SpellBookId, cancellationToken);
        if (spellBook == null)
            throw new SpellBookNotFoundException(request.SpellBookId);

        var existing = await uow.CharacterSpellBooks.GetByCharacterIdAndSpellBookIdAsync(characterId, request.SpellBookId, cancellationToken);
        if (existing != null)
            throw new SpellBookAlreadyLinkedException(request.SpellBookId);

        var characterSpellBook = new CharacterSpellBookEntity
        {
            Character = character,
            SpellBook = spellBook,
            MaxLevel = request.MaxLevel,
        };
        uow.CharacterSpellBooks.Add(characterSpellBook);

        character.AddHistoryEntry(characterHistoryUtil.CreateAddSpellBook(characterId, spellBook, request.MaxLevel));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterAddSpellBook(characterId, characterSpellBook);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();

        return characterSpellBook;
    }

    public async Task<CharacterSpellBookEntity> UpdateSpellBookAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid spellBookId,
        UpdateCharacterSpellBookRequest request,
        CancellationToken cancellationToken = default
    )
    {
        EnsureValidMaxLevel(request.MaxLevel);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);

        var characterSpellBook = await uow.CharacterSpellBooks.GetByCharacterIdAndSpellBookIdAsync(characterId, spellBookId, cancellationToken);
        if (characterSpellBook == null)
            throw new CharacterSpellBookNotFoundException(spellBookId);

        if (characterSpellBook.MaxLevel == request.MaxLevel)
            return characterSpellBook;

        character.AddHistoryEntry(characterHistoryUtil.CreateUpdateSpellBook(characterId, characterSpellBook.SpellBook, characterSpellBook.MaxLevel, request.MaxLevel));
        characterSpellBook.MaxLevel = request.MaxLevel;

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterUpdateSpellBook(characterId, characterSpellBook);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();

        return characterSpellBook;
    }

    public async Task RemoveSpellBookAsync(
        NaheulbookExecutionContext executionContext,
        int characterId,
        Guid spellBookId,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await GetCharacterAsync(uow, executionContext, characterId, cancellationToken);

        var characterSpellBook = await uow.CharacterSpellBooks.GetByCharacterIdAndSpellBookIdAsync(characterId, spellBookId, cancellationToken);
        if (characterSpellBook == null)
            throw new CharacterSpellBookNotFoundException(spellBookId);

        uow.CharacterSpellBooks.Remove(characterSpellBook);
        character.AddHistoryEntry(characterHistoryUtil.CreateRemoveSpellBook(characterId, characterSpellBook.SpellBook));

        var notificationSession = notificationSessionFactory.CreateSession();
        notificationSession.NotifyCharacterRemoveSpellBook(characterId, spellBookId);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();
    }

    private static void EnsureValidMaxLevel(int? maxLevel)
    {
        if (maxLevel is < 1)
            throw new InvalidSpellBookMaxLevelException(maxLevel.Value);
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
