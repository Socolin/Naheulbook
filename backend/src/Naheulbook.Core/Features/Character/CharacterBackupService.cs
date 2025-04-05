using AutoMapper;
using Naheulbook.Core.Features.Character.Backup;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterBackupService
{
    Task<BackupCharacter> GetBackupCharacterAsync(NaheulbookExecutionContext executionContext, int characterId);
}

public class CharacterBackupService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    IMapper mapper
) : ICharacterBackupService
{
    public async Task<BackupCharacter> GetBackupCharacterAsync(NaheulbookExecutionContext executionContext, int characterId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var character = await uow.Characters.GetWithAllDataAsync(characterId);
        if (character == null)
            throw new CharacterNotFoundException(characterId);

        authorizationUtil.EnsureCharacterAccess(executionContext, character);

        return mapper.Map<Backup.V1.BackupCharacter>(character);
    }
}