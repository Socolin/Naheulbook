using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Services;
using Naheulbook.Web.Extensions;

namespace Naheulbook.Web.Hubs
{
    public class ChangeNotifierHub : Hub
    {
        private readonly ICharacterService _characterService;
        private readonly IGroupService _groupService;
        private readonly ILootService _lootService;
        private readonly IMonsterService _monsterService;

        public ChangeNotifierHub(
            ICharacterService characterService,
            IGroupService groupService,
            ILootService lootService,
            IMonsterService monsterService
        )
        {
            _characterService = characterService;
            _groupService = groupService;
            _lootService = lootService;
            _monsterService = monsterService;
        }

        public async Task SubscribeCharacter(int characterId)
        {
            var executionContext = Context.GetHttpContext().GetExecutionContext();
            try
            {
                await _characterService.EnsureUserCanAccessCharacterAsync(executionContext, characterId);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HubException("Access to this resources is forbidden", ex);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"characters:{characterId}");
        }

        public async Task SubscribeGroup(int groupId)
        {
            var executionContext = Context.GetHttpContext().GetExecutionContext();
            try
            {
                await _groupService.EnsureUserCanAccessGroupAsync(executionContext, groupId);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HubException("Access to this resources is forbidden", ex);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"group:{groupId}");
        }

        public async Task SubscribeLoot(int lootId)
        {
            var executionContext = Context.GetHttpContext().GetExecutionContext();
            try
            {
                await _lootService.EnsureUserCanAccessLootAsync(executionContext, lootId);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HubException("Access to this resources is forbidden", ex);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"loots:{lootId}");
        }

        public async Task SubscribeMonster(int monsterId)
        {
            var executionContext = Context.GetHttpContext().GetExecutionContext();
            try
            {
                await _monsterService.EnsureUserCanAccessMonsterAsync(executionContext, monsterId);
            }
            catch (ForbiddenAccessException ex)
            {
                throw new HubException("Access to this resources is forbidden", ex);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"monsters:{monsterId}");
        }
    }
}