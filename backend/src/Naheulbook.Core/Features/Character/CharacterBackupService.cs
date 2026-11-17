using Naheulbook.Core.Features.Character.Backup;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterBackupService
{
    Task<BackupCharacter> GetBackupCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CancellationToken cancellationToken = default);
}

public class CharacterBackupService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    ICharacterBackupMapper characterBackupMapper
) : ICharacterBackupService
{
    public async Task<BackupCharacter> GetBackupCharacterAsync(NaheulbookExecutionContext executionContext, int characterId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithAllDataAsync(characterId, cancellationToken);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        return characterBackupMapper.ToBackup(character);
    }
}