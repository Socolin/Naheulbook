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
        Task<MonsterType> CreateMonsterTypeAsync(NaheulbookExecutionContext executionContext, CreateMonsterTypeRequest request);
        Task<MonsterSubCategory> CreateMonsterSubCategoryAsync(NaheulbookExecutionContext executionContext, int monsterTypeId, CreateMonsterSubCategoryRequest request);
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

        public async Task<MonsterType> CreateMonsterTypeAsync(
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
                    SubCategories = new List<MonsterSubCategory>()
                };

                uow.MonsterTypes.Add(monsterType);
                await uow.SaveChangesAsync();

                return monsterType;
            }
        }

        public async Task<MonsterSubCategory> CreateMonsterSubCategoryAsync(
            NaheulbookExecutionContext executionContext,
            int monsterTypeId,
            CreateMonsterSubCategoryRequest request
        )
        {
            await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var monsterSubCategory = new MonsterSubCategory
                {
                    Name = request.Name,
                    TypeId = monsterTypeId
                };

                uow.MonsterSubCategories.Add(monsterSubCategory);
                await uow.SaveChangesAsync();

                return monsterSubCategory;
            }
        }
    }
}