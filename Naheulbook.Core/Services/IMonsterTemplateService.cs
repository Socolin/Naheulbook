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
        Task<MonsterTemplate> CreateMonsterTemplate(NaheulbookExecutionContext executionContext, CreateMonsterTemplateRequest request);
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

        public async Task<MonsterTemplate> CreateMonsterTemplate(NaheulbookExecutionContext executionContext, CreateMonsterTemplateRequest request)
        {
            await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var category = await uow.MonsterCategories.GetAsync(request.CategoryId);
                if (category == null)
                    throw new MonsterCategoryNotFoundException(request.CategoryId);
                var locations = await uow.Locations.GetByIdsAsync(request.Monster.Locations);
                var itemTemplates = await uow.ItemTemplates.GetByIdsAsync(request.Monster.SimpleInventory.Select(x => x.ItemTemplate.Id));

                var monsterTemplate = new MonsterTemplate
                {
                    Data = JsonConvert.SerializeObject(request.Monster.Data),
                    Name = request.Monster.Name,
                    Category = category,
                    Locations = locations.Select(location => new MonsterLocation
                    {
                        Location = location
                    }).ToList(),
                    Items = request.Monster.SimpleInventory.Select(i => new MonsterTemplateSimpleInventory
                    {
                        Chance = i.Chance,
                        ItemTemplate = itemTemplates.First(x => x.Id == i.ItemTemplate.Id),
                        MaxCount = i.MaxCount,
                        MinCount = i.MinCount
                    }).ToList()
                };

                uow.MonsterTemplates.Add(monsterTemplate);

                await uow.CompleteAsync();

                return monsterTemplate;
            }
        }

    }
}