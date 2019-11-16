using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Newtonsoft.Json;

namespace Naheulbook.Core.Services
{
    public interface IMonsterTemplateService
    {
        Task<MonsterTemplate> CreateMonsterTemplateAsync(NaheulbookExecutionContext executionContext, MonsterTemplateRequest request);
        Task<MonsterTemplate> EditMonsterTemplateAsync(NaheulbookExecutionContext executionContext, int monsterTemplateId, MonsterTemplateRequest request);
        Task<List<MonsterTemplate>> GetAllMonstersAsync();
        Task<List<MonsterTemplate>> SearchMonsterAsync(string filter, int? monsterTypeId, int? monsterSubCategoryId);
    }

    public class MonsterTemplateService : IMonsterTemplateService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;

        public MonsterTemplateService(IUnitOfWorkFactory unitOfWorkFactory, IAuthorizationUtil authorizationUtil)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
        }

        public async Task<MonsterTemplate> CreateMonsterTemplateAsync(NaheulbookExecutionContext executionContext, MonsterTemplateRequest request)
        {
            await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var subCategory = await uow.MonsterSubCategories.GetAsync(request.SubCategoryId);
                if (subCategory == null)
                    throw new MonsterSubCategoryNotFoundException(request.SubCategoryId);
                var itemTemplates = await uow.ItemTemplates.GetByIdsAsync(request.Inventory.Select(x => x.ItemTemplateId));

                var monsterTemplate = new MonsterTemplate
                {
                    Data = JsonConvert.SerializeObject(request.Data),
                    Name = request.Name,
                    SubCategory = subCategory,
                    Items = request.Inventory.Where(i => !i.Id.HasValue || i.Id == 0).Select(i => new MonsterTemplateInventoryElement
                    {
                        Chance = i.Chance,
                        ItemTemplate = itemTemplates.First(x => x.Id == i.ItemTemplateId),
                        MaxCount = i.MaxCount,
                        MinCount = i.MinCount
                    }).ToList()
                };

                uow.MonsterTemplates.Add(monsterTemplate);

                await uow.SaveChangesAsync();

                return monsterTemplate;
            }
        }

        public async Task<MonsterTemplate> EditMonsterTemplateAsync(NaheulbookExecutionContext executionContext, int monsterTemplateId, MonsterTemplateRequest request)
        {
            await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
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
                var newItems = request.Inventory.Where(i => !i.Id.HasValue || i.Id == 0).Select(i => new MonsterTemplateInventoryElement
                {
                    Chance = i.Chance,
                    ItemTemplate = itemTemplates.First(x => x.Id == i.ItemTemplateId),
                    MaxCount = i.MaxCount,
                    MinCount = i.MinCount
                });

                foreach (var item in newItems)
                {
                    monsterTemplate.Items.Add(item);
                }

                await uow.SaveChangesAsync();

                return await uow.MonsterTemplates.GetByIdWithItemsAsync(monsterTemplateId);
            }
        }

        public async Task<List<MonsterTemplate>> GetAllMonstersAsync()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.MonsterTemplates.GetAllWithItemsFullDataAsync();
            }
        }

        public async Task<List<MonsterTemplate>> SearchMonsterAsync(string filter, int? monsterTypeId, int? monsterSubCategoryId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.MonsterTemplates.SearchByNameAndSubCategoryAsync(filter, monsterTypeId, monsterSubCategoryId, 10);
            }
        }
    }
}