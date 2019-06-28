using System.Threading.Tasks;
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
        Task NotifyCharacterSetStatBonusAdAsync(int characterId, string stat);
        Task NotifyCharacterAddModifierAsync(int characterId, ActiveStatsModifier characterModifier);
        Task NotifyCharacterRemoveModifierAsync(int characterId, int characterModifierId);
    }
}