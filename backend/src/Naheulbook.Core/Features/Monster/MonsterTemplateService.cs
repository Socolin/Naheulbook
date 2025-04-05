using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Newtonsoft.Json;

namespace Naheulbook.Core.Features.Monster;

public interface IMonsterTemplateService
{
    Task<MonsterTemplateEntity> CreateMonsterTemplateAsync(NaheulbookExecutionContext executionContext, MonsterTemplateRequest request);
    Task<MonsterTemplateEntity> EditMonsterTemplateAsync(NaheulbookExecutionContext executionContext, int monsterTemplateId, MonsterTemplateRequest request);
    Task<List<MonsterTemplateEntity>> GetAllMonstersAsync();
    Task<List<MonsterTemplateEntity>> SearchMonsterAsync(string filter, int? monsterTypeId, int? monsterSubCategoryId);
}

public class MonsterTemplateService(IUnitOfWorkFactory unitOfWorkFactory, IAuthorizationUtil authorizationUtil)
    : IMonsterTemplateService
{
    public async Task<MonsterTemplateEntity> CreateMonsterTemplateAsync(NaheulbookExecutionContext executionContext, MonsterTemplateRequest request)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var subCategory = await uow.MonsterSubCategories.GetAsync(request.SubCategoryId);
        if (subCategory == null)
            throw new MonsterSubCategoryNotFoundException(request.SubCategoryId);
        var itemTemplates = await uow.ItemTemplates.GetByIdsAsync(request.Inventory.Select(x => x.ItemTemplateId));

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

        await uow.SaveChangesAsync();

        return monsterTemplate;
    }

    public async Task<MonsterTemplateEntity> EditMonsterTemplateAsync(NaheulbookExecutionContext executionContext, int monsterTemplateId, MonsterTemplateRequest request)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var subCategory = await uow.MonsterSubCategories.GetAsync(request.SubCategoryId);
            if (subCategory == null)
                throw new MonsterSubCategoryNotFoundException(request.SubCategoryId);

            var monsterTemplate = await uow.MonsterTemplates.GetByIdWithItemsAsync(monsterTemplateId);
            if (monsterTemplate == null)
                throw new MonsterTemplateNotFoundException(monsterTemplateId);

            var itemTemplates = await uow.ItemTemplates.GetByIdsAsync(request.Inventory.Select(x => x.ItemTemplateId));

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

            await uow.SaveChangesAsync();

            return (await uow.MonsterTemplates.GetByIdWithItemsAsync(monsterTemplateId))!;
        }
    }

    public async Task<List<MonsterTemplateEntity>> GetAllMonstersAsync()
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.MonsterTemplates.GetAllWithItemsFullDataAsync();
        }
    }

    public async Task<List<MonsterTemplateEntity>> SearchMonsterAsync(string filter, int? monsterTypeId, int? monsterSubCategoryId)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return new List<MonsterTemplateEntity>();

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.MonsterTemplates.SearchByNameAndSubCategoryAsync(filter, monsterTypeId, monsterSubCategoryId, 10);
    }
}