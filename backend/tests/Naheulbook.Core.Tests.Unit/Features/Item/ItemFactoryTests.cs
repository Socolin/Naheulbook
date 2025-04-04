using System;
using FluentAssertions;
using Naheulbook.Core.Features.Item;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Item;

public class ItemFactoryTests
{
    private ItemFactory _factory;
    private IJsonUtil _jsonUtil;
    private IItemDataUtil _itemDataUtil;

    [SetUp]
    public void SetUp()
    {
        _jsonUtil = Substitute.For<IJsonUtil>();
        _itemDataUtil = Substitute.For<IItemDataUtil>();
        _factory = new ItemFactory(_jsonUtil, _itemDataUtil);
    }

    [Test]
    public void CreateItemFromRequest_ShouldSerializeJsonData_AndSetItemTemplateId()
    {
        var itemData = new ItemData();
        var jsonItemData = "some-json";
        var itemTemplate = CreateItemTemplate();

        _jsonUtil.DeserializeOrCreate<PartialItemTemplateData>(Arg.Any<string>())
            .Returns(new PartialItemTemplateData());
        _jsonUtil.SerializeNonNull(itemData)
            .Returns(jsonItemData);

        var actualItem = _factory.CreateItem(ItemOwnerType.Character, 10, itemTemplate, itemData);

        actualItem.ItemTemplateId.Should().Be(itemTemplate.Id);
        actualItem.Data.Should().BeEquivalentTo(jsonItemData);
    }


    [Test]
    public void CreateItemFromRequest_ShouldFillChargeIfPresentInItemTemplateData()
    {
        var itemData = new ItemData();
        var itemTemplate = CreateItemTemplate();
        var partialItemTemplateData = new PartialItemTemplateData {Charge = 2};

        _jsonUtil.DeserializeOrCreate<PartialItemTemplateData>("some-item-template-data")
            .Returns(partialItemTemplateData);
        _jsonUtil.SerializeNonNull(Arg.Any<ItemData>())
            .Returns("some-json");
        _jsonUtil.When(x => x.Serialize(itemData))
            .Do(_ => itemData.Charge.Should().Be(2));

        var actualItem = _factory.CreateItem(ItemOwnerType.Character, 10, itemTemplate, itemData);

        actualItem.Data.Should().BeEquivalentTo("some-json");
    }

    [Test]
    public void CreateItemFromRequest_ShouldFillIconIfPresentInItemTemplateData()
    {
        var itemData = new ItemData();
        var itemTemplate = CreateItemTemplate();
        var icon = new JObject();
        var partialItemTemplateData = new PartialItemTemplateData {Icon = icon};

        _jsonUtil.DeserializeOrCreate<PartialItemTemplateData>("some-item-template-data")
            .Returns(partialItemTemplateData);
        _jsonUtil.SerializeNonNull(Arg.Any<ItemData>())
            .Returns("some-json");
        _jsonUtil.When(x => x.Serialize(itemData))
            .Do(_ => itemData.Icon.Should().BeSameAs(icon));

        var actualItem = _factory.CreateItem(ItemOwnerType.Character, 10, itemTemplate, itemData);

        actualItem.Data.Should().BeEquivalentTo("some-json");
    }

    [Test]
    public void CreateItemFromRequest_ShouldFillLifetimeIfPresentInItemTemplateData()
    {
        var itemData = new ItemData();
        var itemTemplate = CreateItemTemplate();
        var lifetime = new JObject();
        var partialItemTemplateData = new PartialItemTemplateData {Lifetime = lifetime};

        _jsonUtil.DeserializeOrCreate<PartialItemTemplateData>("some-item-template-data")
            .Returns(partialItemTemplateData);
        _jsonUtil.SerializeNonNull(Arg.Any<ItemData>())
            .Returns("some-json");
        _jsonUtil.When(x => x.Serialize(itemData))
            .Do(_ => itemData.Lifetime.Should().BeSameAs(lifetime));

        var actualItem = _factory.CreateItem(ItemOwnerType.Character, 10, itemTemplate, itemData);

        actualItem.Data.Should().BeEquivalentTo("some-json");
    }
    [Test]
    public void CreateItemFromRequest_AndOwnerTypeIsCharacter_ShouldSetCharacterId()
    {
        const int characterId = 10;

        _jsonUtil.DeserializeOrCreate<PartialItemTemplateData>(Arg.Any<string>())
            .Returns(new PartialItemTemplateData());

        var actualItem = _factory.CreateItem(ItemOwnerType.Character, characterId, new ItemTemplateEntity(), new ItemData());

        actualItem.CharacterId.Should().Be(characterId);
    }

    [Test]
    public void CreateItemFromRequest_AndOwnerTypeIsLoot_ShouldSetLootId()
    {
        const int lootId = 10;

        _jsonUtil.DeserializeOrCreate<PartialItemTemplateData>(Arg.Any<string>())
            .Returns(new PartialItemTemplateData());

        var actualItem = _factory.CreateItem(ItemOwnerType.Loot, lootId, new ItemTemplateEntity(), new ItemData());

        actualItem.LootId.Should().Be(lootId);
    }

    [Test]
    public void CreateItemFromRequest_AndOwnerTypeIsMonster_ShouldSetMonsterId()
    {
        const int monsterId = 10;

        _jsonUtil.DeserializeOrCreate<PartialItemTemplateData>(Arg.Any<string>())
            .Returns(new PartialItemTemplateData());

        var actualItem = _factory.CreateItem(ItemOwnerType.Monster, monsterId, new ItemTemplateEntity(), new ItemData());

        actualItem.MonsterId.Should().Be(monsterId);
    }

    private ItemTemplateEntity CreateItemTemplate()
    {
        return new ItemTemplateEntity
        {
            Id = Guid.NewGuid(),
            Data = "some-item-template-data",
        };
    }
}