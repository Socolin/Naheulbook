using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface ICharacterService
    {
        Task<List<Character>> GetCharacterListAsync(NaheulbookExecutionContext executionContext);
        Task<Character> CreateCharacterAsync(NaheulbookExecutionContext executionContext, CreateCharacterRequest request);
    }

    public class CharacterService : ICharacterService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ICharacterFactory _characterFactory;

        public CharacterService(
            IUnitOfWorkFactory unitOfWorkFactory,
            ICharacterFactory characterFactory
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _characterFactory = characterFactory;
        }

        public async Task<List<Character>> GetCharacterListAsync(NaheulbookExecutionContext executionContext)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Characters.GetForSummaryByOwnerIdAsync(executionContext.UserId);
            }
        }

        public async Task<Character> CreateCharacterAsync(NaheulbookExecutionContext executionContext, CreateCharacterRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var character = _characterFactory.CreateCharacter(request);

                character.OwnerId = executionContext.UserId;

                uow.Characters.Add(character);
                await uow.CompleteAsync();

                return character;
            }
        }
    }
}