using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.Hubs;
using Naheulbook.Web.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Naheulbook.Web.Services
{
    public class ChangeNotifier : IChangeNotifier
    {
        private enum ElementType
        {
            Character,
            Group,
            Monster,
            Loot
        }

        private readonly IHubContext<ChangeNotifierHub> _hubContext;
        private readonly IHubGroupUtil _hubGroupUtil;
        private readonly IMapper _mapper;
        private static JsonSerializerSettings _jsonSerializerSettings;

        public ChangeNotifier(
            IHubContext<ChangeNotifierHub> hubContext,
            IHubGroupUtil hubGroupUtil,
            IMapper mapper
        )
        {
            _hubContext = hubContext;
            _hubGroupUtil = hubGroupUtil;
            _mapper = mapper;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public Task NotifyCharacterChangeEvAsync(Character character)
        {
            return SendCharacterChangeAsync(character, "update", new {Stat = "ev", Value = character.Ev});
        }

        public Task NotifyCharacterChangeEaAsync(Character character)
        {
            return SendCharacterChangeAsync(character, "update", new {Stat = "ea", Value = character.Ea});
        }

        public Task NotifyCharacterChangeFatePointAsync(Character character)
        {
            return SendCharacterChangeAsync(character, "update", new {Stat = "fatePoint", Value = character.FatePoint});
        }

        public Task NotifyCharacterChangeExperienceAsync(Character character)
        {
            return SendCharacterChangeAsync(character, "update", new {Stat = "experience", Value = character.Experience});
        }

        public Task NotifyCharacterChangeSexAsync(Character character)
        {
            return SendCharacterChangeAsync(character, "update", new {Stat = "sex", Value = character.Sex});
        }

        public Task NotifyCharacterChangeNameAsync(Character character)
        {
            return SendCharacterChangeAsync(character, "update", new {Stat = "name", Value = character.Name});
        }

        public Task NotifyCharacterAddItemAsync(int characterId, Item item)
        {
            return SendCharacterChangeAsync(characterId, "addItem", _mapper.Map<ItemResponse>(item));
        }

        public Task NotifyItemDataChangedAsync(Item item)
        {
            if (item.CharacterId != null)
                return SendCharacterChangeAsync(item.CharacterId.Value, "updateItem", _mapper.Map<ItemResponse>(item));
            if (item.MonsterId != null)
                return SendMonsterChangeAsync(item.MonsterId.Value, "updateItem", _mapper.Map<ItemResponse>(item));
            if (item.LootId != null)
                return SendLootChangeAsync(item.LootId.Value, "updateItem", _mapper.Map<ItemResponse>(item));

            throw new NotSupportedException();
        }

        public Task NotifyItemModifiersChangedAsync(Item item)
        {
            if (item.CharacterId != null)
                return SendCharacterChangeAsync(item.CharacterId.Value, "updateItemModifiers", _mapper.Map<ItemResponse>(item));
            if (item.MonsterId != null)
                return SendMonsterChangeAsync(item.MonsterId.Value, "updateItemModifiers", _mapper.Map<ItemResponse>(item));
            if (item.LootId != null)
                return SendLootChangeAsync(item.LootId.Value, "updateItemModifiers", _mapper.Map<ItemResponse>(item));

            throw new NotSupportedException();
        }

        public Task NotifyEquipItemAsync(Item item)
        {
            if (item.CharacterId != null)
                return SendCharacterChangeAsync(item.CharacterId.Value, "equipItem", _mapper.Map<ItemResponse>(item));
            if (item.MonsterId != null)
                return SendMonsterChangeAsync(item.MonsterId.Value, "equipItem", _mapper.Map<ItemResponse>(item));
            if (item.LootId != null)
                return SendLootChangeAsync(item.LootId.Value, "equipItem", _mapper.Map<ItemResponse>(item));

            throw new NotSupportedException();
        }

        public Task NotifyCharacterSetStatBonusAdAsync(int characterId, string stat)
        {
            return SendCharacterChangeAsync(characterId, "statBonusAd", stat);
        }

        public Task NotifyCharacterAddModifierAsync(int characterId, ActiveStatsModifier characterModifier)
        {
            return SendCharacterChangeAsync(characterId, "addModifier", characterModifier);
        }

        public Task NotifyCharacterRemoveModifierAsync(int characterId, int characterModifierId)
        {
            return SendCharacterChangeAsync(characterId, "removeModifier", characterModifierId);
        }

        public Task NotifyUpdateCharacterModifierAsync(int characterId, ActiveStatsModifier characterModifier)
        {
            return SendCharacterChangeAsync(characterId, "updateModifier", characterModifier);
        }

        public Task NotifyCharacterGroupInviteAsync(int characterId, GroupInvite groupInvite)
        {
            return SendCharacterChangeAsync(characterId, "groupInvite", _mapper.Map<GroupInviteResponse>(groupInvite));
        }

        public Task NotifyGroupCharacterInviteAsync(int groupId, GroupInvite groupInvite)
        {
            return SendGroupChangeAsync(groupId, "groupInvite", _mapper.Map<GroupInviteResponse>(groupInvite));
        }

        private Task SendCharacterChangeAsync(Character character, string action, object data)
        {
            return SendCharacterChangeAsync(character.Id, action, data);
        }

        private Task SendCharacterChangeAsync(int characterId, string action, object data)
        {
            return _hubContext.Clients.Group(_hubGroupUtil.GetCharacterGroupName(characterId))
                .SendAsync("event", GetPacket(ElementType.Character, characterId, action, data));
        }

        private Task SendGroupChangeAsync(Group group, string action, object data)
        {
            return SendGroupChangeAsync(group.Id, action, data);
        }

        private Task SendGroupChangeAsync(int groupId, string action, object data)
        {
            return _hubContext.Clients.Group(_hubGroupUtil.GetGroupGroupName(groupId))
                .SendAsync("event", GetPacket(ElementType.Group, groupId, action, data));
        }

        private Task SendLootChangeAsync(Loot loot, string action, object data)
        {
            return SendLootChangeAsync(loot.Id, action, data);
        }

        private Task SendLootChangeAsync(int lootId, string action, object data)
        {
            return _hubContext.Clients.Group(_hubGroupUtil.GetLootGroupName(lootId))
                .SendAsync("event", GetPacket(ElementType.Loot, lootId, action, data));
        }

        private Task SendMonsterChangeAsync(Monster monster, string action, object data)
        {
            return SendMonsterChangeAsync(monster.Id, action, data);
        }

        private Task SendMonsterChangeAsync(int monsterId, string action, object data)
        {
            return _hubContext.Clients.Group(_hubGroupUtil.GetMonsterGroupName(monsterId))
                .SendAsync("event", GetPacket(ElementType.Monster, monsterId, action, data));
        }

        private static object GetPacket(ElementType elementType, int elementId, string opcode, object data)
        {
            return JsonConvert.SerializeObject(new
            {
                Id = elementId,
                Type = elementType.ToString().ToLowerInvariant(),
                Opcode = opcode,
                Data = data
            }, _jsonSerializerSettings);
        }
    }
}