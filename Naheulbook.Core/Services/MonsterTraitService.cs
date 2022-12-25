using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services
{
    public interface IMonsterTraitService
    {
        Task<List<MonsterTraitEntity>> GetMonsterTraitsAsync();
    }

    public class MonsterTraitService : IMonsterTraitService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public MonsterTraitService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<List<MonsterTraitEntity>> GetMonsterTraitsAsync()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.MonsterTraits.GetAllAsync();
            }
        }
    }
}