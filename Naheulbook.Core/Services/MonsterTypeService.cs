using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services;

public interface IMonsterTypeService
{
    Task<List<MonsterTypeEntity>> GetMonsterTypesWithCategoriesAsync();
    Task<MonsterTypeEntity> CreateMonsterTypeAsync(NaheulbookExecutionContext executionContext, CreateMonsterTypeRequest request);
    Task<MonsterSubCategoryEntity> CreateMonsterSubCategoryAsync(NaheulbookExecutionContext executionContext, int monsterTypeId, CreateMonsterSubCategoryRequest request);
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

    public async Task<List<MonsterTypeEntity>> GetMonsterTypesWithCategoriesAsync()
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.MonsterTypes.GetAllWithCategoriesAsync();
        }
    }

    public async Task<MonsterTypeEntity> CreateMonsterTypeAsync(
        NaheulbookExecutionContext executionContext,
        CreateMonsterTypeRequest request
    )
    {
        await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var monsterType = new MonsterTypeEntity
            {
                Name = request.Name,
                SubCategories = new List<MonsterSubCategoryEntity>()
            };

            uow.MonsterTypes.Add(monsterType);
            await uow.SaveChangesAsync();

            return monsterType;
        }
    }

    public async Task<MonsterSubCategoryEntity> CreateMonsterSubCategoryAsync(
        NaheulbookExecutionContext executionContext,
        int monsterTypeId,
        CreateMonsterSubCategoryRequest request
    )
    {
        await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            var monsterSubCategory = new MonsterSubCategoryEntity
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