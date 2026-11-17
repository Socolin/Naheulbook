using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;
using Newtonsoft.Json;

namespace Naheulbook.Core.Features.Monster;

public interface IMonsterTemplateService
{
    Task<MonsterTemplateEntity> CreateMonsterTemplateAsync(NaheulbookExecutionContext executionContext, MonsterTemplateRequest request, CancellationToken cancellationToken = default);
    Task<MonsterTemplateEntity> EditMonsterTemplateAsync(NaheulbookExecutionContext executionContext, int monsterTemplateId, MonsterTemplateRequest request, CancellationToken cancellationToken = default);
    Task<List<MonsterTemplateEntity>> GetAllMonstersAsync(CancellationToken cancellationToken = default);
    Task<List<MonsterTemplateEntity>> SearchMonsterAsync(string filter, int? monsterTypeId, int? monsterSubCategoryId, CancellationToken cancellationToken = default);
}

public class MonsterTemplateService(IUnitOfWorkFactory unitOfWorkFactory, IAuthorizationUtil authorizationUtil)
    : IMonsterTemplateService
{
    public async Task<MonsterTemplateEntity> CreateMonsterTemplateAsync(NaheulbookExecutionContext executionContext, MonsterTemplateRequest request, CancellationToken cancellationToken = default)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var subCategory = await uow.MonsterSubCategories.GetAsync(request.SubCategoryId, cancellationToken);
        if (subCategory == null)
            throw new MonsterSubCategoryNotFoundException(request.SubCategoryId);
        var itemTemplates = await uow.ItemTemplates.GetByIdsAsync(request.Inventory.Select(x => x.ItemTemplateId), cancellationToken);

        var monsterTemplate = new MonsterTemplateEntity
        {
            Data = JsonConvert.SerializeObject(request.Data),
            Name = request.Name,
            SubCategory = subCategory,
            Items = request.Inventory.Where(i => !i.Id.HasValue || i.Id == 0).Select(i => new MonsterTemplateInventoryElementEntity
                {
                    Chance = i.Chance,
                    ItemTemplate = itemTemplates.First(x => x.Id == i.ItemTemplateId),
                    MaxCount = i.MaxCount,
                    MinCount = i.MinCount,
                }
            ).ToList(),
        };

        uow.MonsterTemplates.Add(monsterTemplate);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return monsterTemplate;
    }

    public async Task<MonsterTemplateEntity> EditMonsterTemplateAsync(NaheulbookExecutionContext executionContext, int monsterTemplateId, MonsterTemplateRequest request, CancellationToken cancellationToken = default)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var subCategory = await uow.MonsterSubCategories.GetAsync(request.SubCategoryId, cancellationToken);
        if (subCategory == null)
            throw new MonsterSubCategoryNotFoundException(request.SubCategoryId);

        var monsterTemplate = await uow.MonsterTemplates.GetByIdWithItemsAsync(monsterTemplateId, cancellationToken);
        if (monsterTemplate == null)
            throw new MonsterTemplateNotFoundException(monsterTemplateId);

        var itemTemplates = await uow.ItemTemplates.GetByIdsAsync(request.Inventory.Select(x => x.ItemTemplateId), cancellationToken);

        monsterTemplate.Data = JsonConvert.SerializeObject(request.Data);
        monsterTemplate.Name = request.Name;

        monsterTemplate.SubCategoryId = subCategory.Id;
        monsterTemplate.SubCategory = subCategory;

        monsterTemplate.Items = monsterTemplate.Items.Where(i => request.Inventory.Any(e => e.Id == i.Id)).ToList();
        var newItems = request.Inventory.Where(i => !i.Id.HasValue || i.Id == 0).Select(i => new MonsterTemplateInventoryElementEntity
            {
                Chance = i.Chance,
                ItemTemplate = itemTemplates.First(x => x.Id == i.ItemTemplateId),
                MaxCount = i.MaxCount,
                MinCount = i.MinCount,
            }
        );

        foreach (var item in newItems)
        {
            monsterTemplate.Items.Add(item);
        }

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return (await uow.MonsterTemplates.GetByIdWithItemsAsync(monsterTemplateId, cancellationToken))!;
    }

    public async Task<List<MonsterTemplateEntity>> GetAllMonstersAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.MonsterTemplates.GetAllWithItemsFullDataAsync(cancellationToken);
    }

    public async Task<List<MonsterTemplateEntity>> SearchMonsterAsync(string filter, int? monsterTypeId, int? monsterSubCategoryId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return new List<MonsterTemplateEntity>();

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.MonsterTemplates.SearchByNameAndSubCategoryAsync(filter, monsterTypeId, monsterSubCategoryId, 10, cancellationToken);
    }
}