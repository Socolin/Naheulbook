using FluentAssertions;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Utils;

public class ItemDataUtilTests
{
    private const string SomeItemDataJson = "some-item-data-json";
    private const string SomeUpdatedJsonData = "some-updated-json-data";
    private IItemDataUtil _util;
    private IJsonUtil _jsonUtil;

    [SetUp]
    public void SetUp()
    {
        _jsonUtil = Substitute.For<IJsonUtil>();

        _util = new ItemDataUtil(_jsonUtil);
    }

    [Test]
    public void IsItemEquipped_ShouldReturnTrueWhenPropertyEquippedIsSet()
    {
        var itemData = new ItemData {Equipped = 1};
        var item = GivenAnItem(itemData);

        _util.IsItemEquipped(item).Should().BeTrue();
    }

    [Test]
    public void IsItemEquipped_ShouldReturnFalseWhenPropertyEquippedIsAbsent()
    {
        var itemData = new ItemData {Equipped = null};
        var item = GivenAnItem(itemData);

        _util.IsItemEquipped(item).Should().BeFalse();
    }

    [Test]
    public void UpdateEquipItem_WhenItemIsNotEquipped_AndNoLevelIsGiven_ShouldSetEquippedTo_1()
    {
        var itemData = new ItemData {Equipped = null};
        var item = GivenAnItem(itemData);

        _util.UpdateEquipItem(item, null);

        itemData.Equipped.Should().Be(1);
        item.Data.Should().Be(SomeUpdatedJsonData);
    }

    [Test]
    public void UpdateEquipItem_WhenItemIsEquipped_AndLevelIs_0_ShouldUnsetEquipped()
    {
        var itemData = new ItemData {Equipped = 1};
        var item = GivenAnItem(itemData);

        _util.UpdateEquipItem(item, 0);

        itemData.Equipped.Should().BeNull();
        item.Data.Should().Be(SomeUpdatedJsonData);
    }

    [Test]
    public void UpdateEquipItem_WhenItemIsEquipped_AndLevelIs_1_ShouldIncrementLevel()
    {
        var itemData = new ItemData {Equipped = 3};
        var item = GivenAnItem(itemData);

        _util.UpdateEquipItem(item, 1);

        itemData.Equipped.Should().Be(4);
        item.Data.Should().Be(SomeUpdatedJsonData);
    }

    [Test]
    public void UpdateEquipItem_WhenItemIsEquipped_AndLevelIs_1_ShouldIncrementBySkipping_0()
    {
        var itemData = new ItemData {Equipped = -1};
        var item = GivenAnItem(itemData);

        _util.UpdateEquipItem(item, 1);

        itemData.Equipped.Should().Be(1);
        item.Data.Should().Be(SomeUpdatedJsonData);
    }

    [Test]
    public void UpdateEquipItem_WhenItemIsEquipped_AndLevelIs_minus_1_ShouldDecrementLevel()
    {
        var itemData = new ItemData {Equipped = -3};
        var item = GivenAnItem(itemData);

        _util.UpdateEquipItem(item, -1);

        itemData.Equipped.Should().Be(-4);
        item.Data.Should().Be(SomeUpdatedJsonData);
    }

    [Test]
    public void UpdateEquipItem_WhenItemIsEquipped_AndLevelIs_minus_1_ShouldDecrementBySkipping_0()
    {
        var itemData = new ItemData {Equipped = 1};
        var item = GivenAnItem(itemData);

        _util.UpdateEquipItem(item, -1);

        itemData.Equipped.Should().Be(-1);
        item.Data.Should().Be(SomeUpdatedJsonData);
    }

    private ItemEntity GivenAnItem(ItemData itemData)
    {
        _jsonUtil.DeserializeOrCreate<ItemData>(SomeItemDataJson)
            .Returns(itemData);
        _jsonUtil.SerializeNonNull(itemData)
            .Returns(SomeUpdatedJsonData);

        return new ItemEntity
        {
            Data = SomeItemDataJson,
        };
    }
}