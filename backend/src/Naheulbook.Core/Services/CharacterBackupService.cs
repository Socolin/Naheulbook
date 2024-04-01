using System.Threading.Tasks;
using AutoMapper;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;

namespace Naheulbook.Core.Services;

public interface ICharacterBackupService
{
    Task<Models.Backup.BackupCharacter> GetBackupCharacterAsync(NaheulbookExecutionContext executionContext, int characterId);
}

public class CharacterBackupService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    IMapper mapper
) : ICharacterBackupService
{
    public async Task<Models.Backup.BackupCharacter> GetBackupCharacterAsync(NaheulbookExecutionContext executionContext, int characterId)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var character = await uow.Characters.GetWithAllDataAsync(characterId);
            if (character == null)
                throw new CharacterNotFoundException(characterId);

            authorizationUtil.EnsureCharacterAccess(executionContext, character);

            return mapper.Map<Models.Backup.V1.BackupCharacter>(character);
        }
    }
}