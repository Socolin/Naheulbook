using System.Linq;
using FluentAssertions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Shared.Utils;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Utils
{
    public class ItemUtilTests
    {
        private ICharacterHistoryUtil _characterHistoryUtil;
        private IItemDataUtil _itemDataUtil;
        private IJsonUtil _jsonUtil;
        private IItemFactory _itemFactory;
        private IItemUtil _util;

        [SetUp]
        public void SetUp()
        {
            _characterHistoryUtil = Substitute.For<ICharacterHistoryUtil>();
            _itemDataUtil = Substitute.For<IItemDataUtil>();
            _jsonUtil = Substitute.For<IJsonUtil>();
            _itemFactory = Substitute.For<IItemFactory>();

            _util = new ItemUtil(
                _characterHistoryUtil,
                _itemDataUtil,
                _jsonUtil,
                new FakeUnitOfWorkFactory(),
                new FakeNotificationSessionFactory(),
                _itemFactory
            );
        }

        [Test]
        public void EquipItem_ShouldUpdateItemData()
        {
            var item = new Item();

            _util.EquipItem(item, 8);

            _itemDataUtil.UpdateEquipItem(item, 8);
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

            _characterHistoryUtil.CreateLogEquipItem(characterId, item)
                .Returns(new CharacterHistoryEntry {Action = "equip"});
            _characterHistoryUtil.CreateLogUnEquipItem(characterId, item)
                .Returns(new CharacterHistoryEntry {Action = "unEquip"});
            _itemDataUtil.IsItemEquipped(item)
                .Returns(wasEquipped, isNowEquipped);

            _util.EquipItem(item, 8);

            item.Character.HistoryEntries?.Last()?.Action.Should().BeEquivalentTo(expectedAction);
        }
    }
}