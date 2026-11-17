using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Monster;

public interface IMonsterTypeService
{
    Task<List<MonsterTypeEntity>> GetMonsterTypesWithCategoriesAsync(CancellationToken cancellationToken = default);
    Task<MonsterTypeEntity> CreateMonsterTypeAsync(NaheulbookExecutionContext executionContext, CreateMonsterTypeRequest request, CancellationToken cancellationToken = default);
    Task<MonsterSubCategoryEntity> CreateMonsterSubCategoryAsync(NaheulbookExecutionContext executionContext, int monsterTypeId, CreateMonsterSubCategoryRequest request, CancellationToken cancellationToken = default);
}

public class MonsterTypeService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil
) : IMonsterTypeService
{
    public async Task<List<MonsterTypeEntity>> GetMonsterTypesWithCategoriesAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.MonsterTypes.GetAllWithCategoriesAsync(cancellationToken);
    }

    public async Task<MonsterTypeEntity> CreateMonsterTypeAsync(
        NaheulbookExecutionContext executionContext,
        CreateMonsterTypeRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monsterType = new MonsterTypeEntity
        {
            Name = request.Name,
            SubCategories = new List<MonsterSubCategoryEntity>(),
        };

        uow.MonsterTypes.Add(monsterType);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return monsterType;
    }

    public async Task<MonsterSubCategoryEntity> CreateMonsterSubCategoryAsync(
        NaheulbookExecutionContext executionContext,
        int monsterTypeId,
        CreateMonsterSubCategoryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var monsterSubCategory = new MonsterSubCategoryEntity
        {
            Name = request.Name,
            TypeId = monsterTypeId,
        };

        uow.MonsterSubCategories.Add(monsterSubCategory);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return monsterSubCategory;
    }
}