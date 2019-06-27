using Naheulbook.Core.Constants;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Utils
{
    public interface IItemDataUtil
    {
        bool IsItemEquipped(JObject itemData);
        void UpdateEquipItem(JObject itemData, int? level);
    }

    public class ItemDataUtil : IItemDataUtil
    {
        public bool IsItemEquipped(JObject itemData)
        {
            return itemData.ContainsKey(ItemDataConstants.EquippedKey);
        }

        public void UpdateEquipItem(JObject itemData, int? level)
        {
            if (!itemData.ContainsKey(ItemDataConstants.EquippedKey))
            {
                itemData[ItemDataConstants.EquippedKey] = 1;
            }

            if (level == 1)
            {
                itemData[ItemDataConstants.EquippedKey] = itemData.Value<int>(ItemDataConstants.EquippedKey) + 1;
                if (itemData.Value<int>(ItemDataConstants.EquippedKey) == 0)
                    itemData[ItemDataConstants.EquippedKey] = 1;
            }
            else if (level == -1)
            {
                itemData[ItemDataConstants.EquippedKey] = itemData.Value<int>(ItemDataConstants.EquippedKey) - 1;
                if (itemData.Value<int>(ItemDataConstants.EquippedKey) == 0)
                    itemData[ItemDataConstants.EquippedKey] = -1;
            }
            else if (level == 0)
            {
                itemData.Remove(ItemDataConstants.EquippedKey);
            }
        }
    }
}