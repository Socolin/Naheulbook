using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Shared.Utils;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Utils
{
    public interface IItemUtil
    {
        void EquipItem(Item item, int? level);
    }

    public class ItemUtil : IItemUtil
    {
        private readonly IItemDataUtil _itemDataUtil;
        private readonly ICharacterHistoryUtil _characterHistoryUtil;
        private readonly IJsonUtil _jsonUtil;

        public ItemUtil(
            ICharacterHistoryUtil characterHistoryUtil,
            IItemDataUtil itemDataUtil,
            IJsonUtil jsonUtil
        )
        {
            _characterHistoryUtil = characterHistoryUtil;
            _itemDataUtil = itemDataUtil;
            _jsonUtil = jsonUtil;
        }

        public void EquipItem(Item item, int? level)
        {
            var itemData = _jsonUtil.Deserialize<JObject>(item.Data);

            var previouslyEquipped = _itemDataUtil.IsItemEquipped(itemData);
            _itemDataUtil.UpdateEquipItem(itemData, level);
            var newlyEquipped = _itemDataUtil.IsItemEquipped(itemData);

            if (previouslyEquipped != newlyEquipped && item.CharacterId != null)
            {
                if (item.Character.HistoryEntries == null)
                    item.Character.HistoryEntries = new List<CharacterHistoryEntry>();
                if (previouslyEquipped)
                    item.Character.HistoryEntries.Add(_characterHistoryUtil.CreateLogUnEquipItem(item.CharacterId.Value, item.Id));
                else
                    item.Character.HistoryEntries.Add(_characterHistoryUtil.CreateLogEquipItem(item.CharacterId.Value, item.Id));
            }

            item.Data = _jsonUtil.Serialize(itemData);
        }
    }
}