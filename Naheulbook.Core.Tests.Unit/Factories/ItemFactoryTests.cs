using FluentAssertions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Factories
{
    public class ItemFactoryTests
    {
        private ItemFactory _factory;
        private IJsonUtil _jsonUtil;

        [SetUp]
        public void SetUp()
        {
            _jsonUtil = Substitute.For<IJsonUtil>();
            _factory = new ItemFactory(_jsonUtil);
        }

        [Test]
        public void CreateItemFromRequest_ShouldSerializeJsonData_AndSetItemTemplateId()
        {
            var itemData = new ItemData();
            var jsonItemData = "some-json";
            var itemTemplate = CreateItemTemplate();

            _jsonUtil.Serialize(itemData)
                .Returns(jsonItemData);

            var actualItem = _factory.CreateItemFromRequest(ItemOwnerType.Character, 10, itemTemplate, itemData);

            actualItem.ItemTemplateId.Should().Be(itemTemplate.Id);
            actualItem.Data.Should().BeEquivalentTo(jsonItemData);
        }


        [Test]
        public void CreateItemFromRequest_ShouldFillChargeIfPresentInItemTemplateData()
        {
            var itemData = new ItemData();
            var itemTemplate = CreateItemTemplate();
            var partialItemTemplateData = new PartialItemTemplateData {Charge = 2};

            _jsonUtil.Deserialize<PartialItemTemplateData>("some-item-template-data")
                .Returns(partialItemTemplateData);
            _jsonUtil.Serialize(Arg.Any<ItemData>())
                .Returns("some-json");
            _jsonUtil.When(x => x.Serialize(itemData))
                .Do(x => itemData.Charge.Should().Be(2));

            var actualItem = _factory.CreateItemFromRequest(ItemOwnerType.Character, 10, itemTemplate, itemData);

            actualItem.Data.Should().BeEquivalentTo("some-json");
        }

        [Test]
        public void CreateItemFromRequest_ShouldFillIconIfPresentInItemTemplateData()
        {
            var itemData = new ItemData();
            var itemTemplate = CreateItemTemplate();
            var icon = new JObject();
            var partialItemTemplateData = new PartialItemTemplateData {Icon = icon};

            _jsonUtil.Deserialize<PartialItemTemplateData>("some-item-template-data")
                .Returns(partialItemTemplateData);
            _jsonUtil.Serialize(Arg.Any<ItemData>())
                .Returns("some-json");
            _jsonUtil.When(x => x.Serialize(itemData))
                .Do(x => itemData.Icon.Should().BeSameAs(icon));

            var actualItem = _factory.CreateItemFromRequest(ItemOwnerType.Character, 10, itemTemplate, itemData);

            actualItem.Data.Should().BeEquivalentTo("some-json");
        }

        [Test]
        public void CreateItemFromRequest_ShouldFillLifetimeIfPresentInItemTemplateData()
        {
            var itemData = new ItemData();
            var itemTemplate = CreateItemTemplate();
            var lifetime = new JObject();
            var partialItemTemplateData = new PartialItemTemplateData {Lifetime = lifetime};

            _jsonUtil.Deserialize<PartialItemTemplateData>("some-item-template-data")
                .Returns(partialItemTemplateData);
            _jsonUtil.Serialize(Arg.Any<ItemData>())
                .Returns("some-json");
            _jsonUtil.When(x => x.Serialize(itemData))
                .Do(x => itemData.Lifetime.Should().BeSameAs(lifetime));

            var actualItem = _factory.CreateItemFromRequest(ItemOwnerType.Character, 10, itemTemplate, itemData);

            actualItem.Data.Should().BeEquivalentTo("some-json");
        }
        [Test]
        public void CreateItemFromRequest_AndOwnerTypeIsCharacter_ShouldSetCharacterId()
        {
            const int characterId = 10;

            var actualItem = _factory.CreateItemFromRequest(ItemOwnerType.Character, characterId, new ItemTemplate(), new ItemData());

            actualItem.CharacterId.Should().Be(characterId);
        }

        [Test]
        public void CreateItemFromRequest_AndOwnerTypeIsLoot_ShouldSetLootId()
        {
            const int lootId = 10;
            var actualItem = _factory.CreateItemFromRequest(ItemOwnerType.Loot, lootId, new ItemTemplate(), new ItemData());

            actualItem.LootId.Should().Be(lootId);
        }

        [Test]
        public void CreateItemFromRequest_AndOwnerTypeIsMonster_ShouldSetMonsterId()
        {
            const int monsterId = 10;

            var actualItem = _factory.CreateItemFromRequest(ItemOwnerType.Monster, monsterId, new ItemTemplate(), new ItemData());

            actualItem.MonsterId.Should().Be(monsterId);
        }

        private ItemTemplate CreateItemTemplate()
        {
            return new ItemTemplate
            {
                Id = RngHelper.GetRandomInt(),
                Data = "some-item-template-data"
            };
        }
    }
}