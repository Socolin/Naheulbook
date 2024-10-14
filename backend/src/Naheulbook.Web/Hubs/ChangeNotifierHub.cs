using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Services;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Hubs;

public class ChangeNotifierHub(
    IHubGroupUtil hubGroupUtil,
    ICharacterService characterService,
    IGroupService groupService,
    ILootService lootService,
    IMonsterService monsterService
) : Hub
{
    public async Task SubscribeCharacter(int characterId)
    {
        var executionContext = GetHttpContext().GetExecutionContext();
        try
        {
            var isGroupMaster = await characterService.EnsureUserCanAccessCharacterAndGetIfIsGroupMasterAsync(executionContext, characterId);
            if (isGroupMaster)
                await Groups.AddToGroupAsync(Context.ConnectionId, hubGroupUtil.GetGmCharacterGroupName(characterId));
            await Groups.AddToGroupAsync(Context.ConnectionId, hubGroupUtil.GetCharacterGroupName(characterId));
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HubException("Access to this resources is forbidden", ex);
        }
    }

    public async Task SubscribeGroup(int groupId)
    {
        var executionContext = GetHttpContext().GetExecutionContext();
        try
        {
            await groupService.EnsureUserCanAccessGroupAsync(executionContext, groupId);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HubException("Access to this resources is forbidden", ex);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, hubGroupUtil.GetGroupGroupName(groupId));
    }

    public async Task SubscribeLoot(int lootId)
    {
        var executionContext = GetHttpContext().GetExecutionContext();
        try
        {
            await lootService.EnsureUserCanAccessLootAsync(executionContext, lootId);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HubException("Access to this resources is forbidden", ex);
        }
        catch (LootNotFoundException ex)
        {
            throw new HubException("Loot not found", ex);
        }
        catch (GroupNotFoundException ex)
        {
            throw new HubException("Group not found", ex);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, hubGroupUtil.GetLootGroupName(lootId));
    }

    public async Task SubscribeMonster(int monsterId)
    {
        var executionContext = GetHttpContext().GetExecutionContext();
        try
        {
            await monsterService.EnsureUserCanAccessMonsterAsync(executionContext, monsterId);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HubException("Access to this resources is forbidden", ex);
        }
        catch (MonsterNotFoundException ex)
        {
            throw new HubException("Monster not found", ex);
        }
        catch (GroupNotFoundException ex)
        {
            throw new HubException("Group not found", ex);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, hubGroupUtil.GetMonsterGroupName(monsterId));
    }

    public async Task SubscribeFight(int fightId)
    {
        var executionContext = GetHttpContext().GetExecutionContext();
        try
        {
            await monsterService.EnsureUserCanAccessFightAsync(executionContext, fightId);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HubException("Access to this resources is forbidden", ex);
        }
        catch (FightNotFoundException ex)
        {
            throw new HubException("Fight not found", ex);
        }
        catch (GroupNotFoundException ex)
        {
            throw new HubException("Group not found", ex);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, hubGroupUtil.GetFightGroupName(fightId));
    }

    public async Task UnsubscribeCharacter(int characterId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, hubGroupUtil.GetCharacterGroupName(characterId));
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, hubGroupUtil.GetGmCharacterGroupName(characterId));
    }

    public async Task UnsubscribeGroup(int groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, hubGroupUtil.GetGroupGroupName(groupId));
    }

    public async Task UnsubscribeLoot(int lootId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, hubGroupUtil.GetLootGroupName(lootId));
    }

    public async Task UnsubscribeMonster(int monsterId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, hubGroupUtil.GetMonsterGroupName(monsterId));
    }

    public async Task UnsubscribeFight(int fightId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, hubGroupUtil.GetMonsterGroupName(fightId));
    }

    private HttpContext GetHttpContext()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
        {
            throw new HubException("httpContext not available");
        }

        return httpContext;
    }
}