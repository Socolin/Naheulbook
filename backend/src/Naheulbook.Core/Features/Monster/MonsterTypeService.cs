using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Features.Monster;

public interface IMonsterTypeService
{
    Task<List<MonsterTypeEntity>> GetMonsterTypesWithCategoriesAsync();
    Task<MonsterTypeEntity> CreateMonsterTypeAsync(NaheulbookExecutionContext executionContext, CreateMonsterTypeRequest request);
    Task<MonsterSubCategoryEntity> CreateMonsterSubCategoryAsync(NaheulbookExecutionContext executionContext, int monsterTypeId, CreateMonsterSubCategoryRequest request);
}

public class MonsterTypeService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil
) : IMonsterTypeService
{
    public async Task<List<MonsterTypeEntity>> GetMonsterTypesWithCategoriesAsync()
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.MonsterTypes.GetAllWithCategoriesAsync();
    }

    public async Task<MonsterTypeEntity> CreateMonsterTypeAsync(
        NaheulbookExecutionContext executionContext,
        CreateMonsterTypeRequest request
    )
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monsterType = new MonsterTypeEntity
        {
            Name = request.Name,
            SubCategories = new List<MonsterSubCategoryEntity>(),
        };

        uow.MonsterTypes.Add(monsterType);
        await uow.SaveChangesAsync();

        return monsterType;
    }

    public async Task<MonsterSubCategoryEntity> CreateMonsterSubCategoryAsync(
        NaheulbookExecutionContext executionContext,
        int monsterTypeId,
        CreateMonsterSubCategoryRequest request
    )
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monsterSubCategory = new MonsterSubCategoryEntity
        {
            Name = request.Name,
            TypeId = monsterTypeId,
        };

        uow.MonsterSubCategories.Add(monsterSubCategory);
        await uow.SaveChangesAsync();

        return monsterSubCategory;
    }
}