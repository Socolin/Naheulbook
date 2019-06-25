using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
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
            return _hubContext.Clients.Group(_hubGroupUtil.GetGroupGroupName(group.Id))
                .SendAsync("event", GetPacket(ElementType.Group, group.Id, action, data));
        }

        private Task SendLootChangeAsync(Loot loot, string action, object data)
        {
            return _hubContext.Clients.Group(_hubGroupUtil.GetLootGroupName(loot.Id))
                .SendAsync("event", GetPacket(ElementType.Loot, loot.Id, action, data));
        }

        private Task SendMonsterChangeAsync(Monster monster, string action, object data)
        {
            return _hubContext.Clients.Group(_hubGroupUtil.GetMonsterGroupName(monster.Id))
                .SendAsync("event", GetPacket(ElementType.Monster, monster.Id, action, data));
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