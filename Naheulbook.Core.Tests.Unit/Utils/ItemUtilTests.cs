using System.Linq;
using FluentAssertions;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Shared.Utils;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Utils
{
    public class ItemUtilTests
    {
        private ICharacterHistoryUtil _characterHistoryUtil;
        private IItemDataUtil _itemDataUtil;
        private IJsonUtil _jsonUtil;
        private IItemUtil _util;

        [SetUp]
        public void SetUp()
        {
            _characterHistoryUtil = Substitute.For<ICharacterHistoryUtil>();
            _itemDataUtil = Substitute.For<IItemDataUtil>();
            _jsonUtil = Substitute.For<IJsonUtil>();

            _util = new ItemUtil(
                _characterHistoryUtil,
                _itemDataUtil,
                _jsonUtil
            );
        }

        [Test]
        public void EquipItem_ShouldUpdateItemData()
        {
            const string itemDataJson = "some-item-data-json";
            const string updatedItemDataJson = "some-updated-item-data-json";
            var itemData = new JObject();
            var item = new Item {Data = itemDataJson};

            _jsonUtil.Deserialize<JObject>(itemDataJson)
                .Returns(itemData);
            _jsonUtil.Serialize(itemData)
                .Returns(updatedItemDataJson);

            _util.EquipItem(item, 8);

            Received.InOrder(() =>
            {
                _itemDataUtil.UpdateEquipItem(itemData, 8);
                _jsonUtil.Serialize(itemData);
            });
            item.Data.Should().BeEquivalentTo(updatedItemDataJson);
        }

        [Test]
        [TestCase(true, true, null)]
        [TestCase(false, false, null)]
        [TestCase(false, true, "equip")]
        [TestCase(true, false, "unEquip")]
        public void EquipItem_ShouldAddACharacterLogWhenItemIsOwnedToACharacterAndStateChange(bool wasEquipped, bool isNowEquipped, string expectedAction)
        {
            const int characterId = 8;
            const int itemId = 12;
            var item = new Item {Id = itemId, CharacterId = characterId, Character = new Character()};
            var itemData = new JObject();

            _jsonUtil.Deserialize<JObject>(Arg.Any<string>())
                .Returns(itemData);
            _jsonUtil.Serialize(Arg.Any<JObject>())
                .Returns("some-json");
            _characterHistoryUtil.CreateLogEquipItem(characterId, itemId)
                .Returns(new CharacterHistoryEntry {Action = "equip"});
            _characterHistoryUtil.CreateLogUnEquipItem(characterId, itemId)
                .Returns(new CharacterHistoryEntry {Action = "unEquip"});
            _itemDataUtil.IsItemEquipped(itemData)
                .Returns(wasEquipped, isNowEquipped);

            _util.EquipItem(item, 8);

            item.Character.HistoryEntries?.Last()?.Action.Should().BeEquivalentTo(expectedAction);
        }
    }
}