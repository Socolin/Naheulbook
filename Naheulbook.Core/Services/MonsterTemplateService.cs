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
                var category = await uow.MonsterCategories.GetAsync(request.CategoryId);
                if (category == null)
                    throw new MonsterCategoryNotFoundException(request.CategoryId);
                var locations = await uow.Locations.GetByIdsAsync(request.LocationIds);
                var itemTemplates = await uow.ItemTemplates.GetByIdsAsync(request.SimpleInventory.Select(x => x.ItemTemplateId));

                var monsterTemplate = new MonsterTemplate
                {
                    Data = JsonConvert.SerializeObject(request.Data),
                    Name = request.Name,
                    Category = category,
                    Locations = locations.Select(location => new MonsterLocation
                    {
                        Location = location
                    }).ToList(),
                    Items = request.SimpleInventory.Where(i => !i.Id.HasValue || i.Id == 0).Select(i => new MonsterTemplateSimpleInventory
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
                var category = await uow.MonsterCategories.GetAsync(request.CategoryId);
                if (category == null)
                    throw new MonsterCategoryNotFoundException(request.CategoryId);

                var monsterTemplate = await uow.MonsterTemplates.GetByIdWithItemsWithLocationsAsync(monsterTemplateId);
                if (monsterTemplate == null)
                    throw new MonsterTemplateNotFoundException(monsterTemplateId);

                var locations = await uow.Locations.GetByIdsAsync(request.LocationIds);
                var itemTemplates = await uow.ItemTemplates.GetByIdsAsync(request.SimpleInventory.Select(x => x.ItemTemplateId));

                monsterTemplate.Data = JsonConvert.SerializeObject(request.Data);
                monsterTemplate.Name = request.Name;

                monsterTemplate.CategoryId = category.Id;
                monsterTemplate.Category = category;

                monsterTemplate.Locations = locations.Select(location => new MonsterLocation
                {
                    Location = location
                }).ToList();
                monsterTemplate.Items = monsterTemplate.Items.Where(i => request.SimpleInventory.Any(e => e.Id == i.Id)).ToList();
                var newItems = request.SimpleInventory.Where(i => !i.Id.HasValue || i.Id == 0).Select(i => new MonsterTemplateSimpleInventory
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

                return await uow.MonsterTemplates.GetByIdWithItemsWithLocationsAsync(monsterTemplateId);
            }
        }

        public async Task<List<MonsterTemplate>> GetAllMonstersAsync()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.MonsterTemplates.GetAllWithItemsFullDataWithLocationsAsync();
            }
        }

        public async Task<List<MonsterTemplate>> SearchMonsterAsync(string filter, int? monsterTypeId, int? monsterSubCategoryId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.MonsterTemplates.SearchByNameAndCategoryAsync(filter, monsterTypeId, monsterSubCategoryId, 10);
            }
        }
    }
}