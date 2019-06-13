using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services
{
    public interface IMonsterTypeService
    {
        Task<List<MonsterType>> GetMonsterTypesWithCategoriesAsync();
    }

    public class MonsterTypeService : IMonsterTypeService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public MonsterTypeService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<List<MonsterType>> GetMonsterTypesWithCategoriesAsync()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.MonsterTypes.GetAllWithCategoriesAsync();
            }
        }
    }
}