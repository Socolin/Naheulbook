using System.Collections.Generic;
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
    public interface IMonsterService
    {
        Task<Monster> CreateMonsterAsync(NaheulbookExecutionContext executionContext, int groupId, CreateMonsterRequest request);
        Task<List<Monster>> GetMonstersForGroupAsync(NaheulbookExecutionContext executionContext, int groupId);
    }

    public class MonsterService : IMonsterService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IActiveStatsModifierUtil _activeStatsModifierUtil;

        public MonsterService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil,
            IActiveStatsModifierUtil activeStatsModifierUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
            _activeStatsModifierUtil = activeStatsModifierUtil;
        }

        public async Task<Monster> CreateMonsterAsync(NaheulbookExecutionContext executionContext, int groupId, CreateMonsterRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                _activeStatsModifierUtil.InitializeModifierIds(request.Modifiers);

                var monster = new Monster
                {
                    Group = group,
                    Name = request.Name,
                    Data = JsonConvert.SerializeObject(request.Data),
                    Modifiers = JsonConvert.SerializeObject(request.Modifiers)
                };

                uow.Monsters.Add(monster);
                await uow.CompleteAsync();

                return monster;
            }
        }

        public async Task<List<Monster>> GetMonstersForGroupAsync(NaheulbookExecutionContext executionContext, int groupId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                return await uow.Monsters.GetByGroupIdWithInventoryAsync(groupId);
            }
        }
    }
}