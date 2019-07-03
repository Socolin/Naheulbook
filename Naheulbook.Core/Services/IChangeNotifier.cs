using System.Threading.Tasks;
using Naheulbook.Core.Models;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Services
{
    public interface IChangeNotifier
    {
        Task NotifyCharacterChangeEvAsync(Character character);
        Task NotifyCharacterChangeEaAsync(Character character);
        Task NotifyCharacterChangeFatePointAsync(Character character);
        Task NotifyCharacterChangeExperienceAsync(Character character);
        Task NotifyCharacterChangeSexAsync(Character character);
        Task NotifyCharacterChangeNameAsync(Character character);
        Task NotifyCharacterAddItemAsync(int characterId, Item item);

        Task NotifyItemDataChangedAsync(Item item);
        Task NotifyItemModifiersChangedAsync(Item item);
        Task NotifyEquipItemAsync(Item item);
        Task NotifyItemChangeContainer(Item item);

        Task NotifyCharacterSetStatBonusAdAsync(int characterId, string stat);
        Task NotifyCharacterAddModifierAsync(int characterId, ActiveStatsModifier characterModifier);
        Task NotifyCharacterRemoveModifierAsync(int characterId, int characterModifierId);
        Task NotifyUpdateCharacterModifierAsync(int characterId, ActiveStatsModifier characterModifier);
        Task NotifyCharacterGroupInviteAsync(int characterId, GroupInvite groupInvite);
        Task NotifyCharacterCancelGroupInviteAsync(int characterId, GroupInvite groupInvite);
        Task NotifyCharacterAcceptGroupInviteAsync(int characterId, GroupInvite groupInvite);
        Task NotifyCharacterShowLootAsync(int characterId, Loot loot);
        Task NotifyCharacterHideLootAsync(int characterId, int lootId);
        Task NotifyGroupCharacterInviteAsync(int groupId, GroupInvite groupInvite);
        Task NotifyGroupCancelGroupInviteAsync(int groupId, GroupInvite groupInvite);
        Task NotifyGroupAcceptGroupInviteAsync(int groupId, GroupInvite groupInvite);
        Task NotifyGroupChangeGroupDataAsync(int groupId, GroupData groupData);
        Task NotifyGroupChangeLocationAsync(int groupId, Location location);
        Task NotifyGroupAddLoot(int groupId, Loot loot);

        Task NotifyLootUpdateVisibilityAsync(int lootId, bool visibleForPlayer);
    }
}