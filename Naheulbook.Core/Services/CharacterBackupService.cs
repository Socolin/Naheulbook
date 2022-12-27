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

public class CharacterBackupService : ICharacterBackupService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IAuthorizationUtil _authorizationUtil;
    private readonly IMapper _mapper;

    public CharacterBackupService(
        IUnitOfWorkFactory unitOfWorkFactory,
        IAuthorizationUtil authorizationUtil,
        IMapper mapper
    )
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _authorizationUtil = authorizationUtil;
        _mapper = mapper;
    }

    public async Task<Models.Backup.BackupCharacter> GetBackupCharacterAsync(NaheulbookExecutionContext executionContext, int characterId)
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var character = await uow.Characters.GetWithAllDataAsync(characterId);
            if (character == null)
                throw new CharacterNotFoundException(characterId);

            _authorizationUtil.EnsureCharacterAccess(executionContext, character);

            return _mapper.Map<Models.Backup.V1.BackupCharacter>(character);
        }
    }
}