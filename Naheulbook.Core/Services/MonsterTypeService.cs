using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface IMonsterTypeService
    {
        Task<List<MonsterType>> GetMonsterTypesWithCategoriesAsync();
        Task<MonsterType> CreateMonsterType(NaheulbookExecutionContext executionContext, CreateMonsterTypeRequest request);
        Task<MonsterCategory> CreateMonsterCategoryAsync(NaheulbookExecutionContext executionContext, int monsterTypeId, CreateMonsterCategoryRequest request);
    }

    public class MonsterTypeService : IMonsterTypeService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;

        public MonsterTypeService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
        }

        public async Task<List<MonsterType>> GetMonsterTypesWithCategoriesAsync()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.MonsterTypes.GetAllWithCategoriesAsync();
            }
        }

        public async Task<MonsterType> CreateMonsterType(
            NaheulbookExecutionContext executionContext,
            CreateMonsterTypeRequest request
        )
        {
            await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monsterType = new MonsterType
                {
                    Name = request.Name,
                    Categories = new List<MonsterCategory>()
                };

                uow.MonsterTypes.Add(monsterType);
                await uow.CompleteAsync();

                return monsterType;
            }
        }

        public async Task<MonsterCategory> CreateMonsterCategoryAsync(
            NaheulbookExecutionContext executionContext,
            int monsterTypeId,
            CreateMonsterCategoryRequest request
        )
        {
            await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monsterCategory = new MonsterCategory
                {
                    Name = request.Name,
                    TypeId = monsterTypeId
                };

                uow.MonsterCategories.Add(monsterCategory);
                await uow.CompleteAsync();

                return monsterCategory;
            }
        }
    }
}