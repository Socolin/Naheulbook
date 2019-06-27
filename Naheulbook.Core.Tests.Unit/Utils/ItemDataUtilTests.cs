using FluentAssertions;
using Naheulbook.Core.Constants;
using Naheulbook.Core.Utils;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Utils
{
    public class ItemDataUtilTests
    {
        private IItemDataUtil _util;

        [SetUp]
        public void SetUp()
        {
            _util = new ItemDataUtil();
        }

        [Test]
        public void IsItemEquipped_ShouldReturnTrueWhenPropertyEquippedIsSet()
        {
            _util.IsItemEquipped(new JObject{[ItemDataConstants.EquippedKey] = 1}).Should().BeTrue();
        }

        [Test]
        public void IsItemEquipped_ShouldReturnFalseWhenPropertyEquippedIsAbsent()
        {
            _util.IsItemEquipped(new JObject()).Should().BeFalse();
        }

        [Test]
        public void UpdateEquipItem_WhenItemIsNotEquipped_AndNoLevelIsGiven_ShouldSetEquippedTo_1()
        {
            var itemData = new JObject();

            _util.UpdateEquipItem(itemData, null);

            itemData.Value<int>(ItemDataConstants.EquippedKey).Should().Be(1);
        }

        [Test]
        public void UpdateEquipItem_WhenItemIsEquipped_AndLevelIs_0_ShouldUnsetEquipped()
        {
            var itemData = new JObject {[ItemDataConstants.EquippedKey] = 1};

            _util.UpdateEquipItem(itemData, 0);

            itemData.ContainsKey(ItemDataConstants.EquippedKey).Should().BeFalse();
        }

        [Test]
        public void UpdateEquipItem_WhenItemIsEquipped_AndLevelIs_1_ShouldIncrementLevel()
        {
            var itemData = new JObject {[ItemDataConstants.EquippedKey] = 3};

            _util.UpdateEquipItem(itemData, 1);

            itemData.Value<int>(ItemDataConstants.EquippedKey).Should().Be(4);
        }

        [Test]
        public void UpdateEquipItem_WhenItemIsEquipped_AndLevelIs_1_ShouldIncrementBySkipping_0()
        {
            var itemData = new JObject {[ItemDataConstants.EquippedKey] = -1};

            _util.UpdateEquipItem(itemData, 1);

            itemData.Value<int>(ItemDataConstants.EquippedKey).Should().Be(1);
        }

        [Test]
        public void UpdateEquipItem_WhenItemIsEquipped_AndLevelIs_minus_1_ShouldDecrementLevel()
        {
            var itemData = new JObject {[ItemDataConstants.EquippedKey] = -3};

            _util.UpdateEquipItem(itemData, -1);

            itemData.Value<int>(ItemDataConstants.EquippedKey).Should().Be(-4);
        }

        [Test]
        public void UpdateEquipItem_WhenItemIsEquipped_AndLevelIs_minus_1_ShouldDecrementBySkipping_0()
        {
            var itemData = new JObject {[ItemDataConstants.EquippedKey] = 1};

            _util.UpdateEquipItem(itemData, -1);

            itemData.Value<int>(ItemDataConstants.EquippedKey).Should().Be(-1);
        }
    }
}