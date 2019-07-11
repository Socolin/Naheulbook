using FluentAssertions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Requests.Requests;
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
            var itemData = JObject.FromObject(new {key = "value"});
            var jsonItemData = "some-json";

            var request = new CreateItemRequest
            {
                ItemData = itemData,
                ItemTemplateId = 12
            };

            _jsonUtil.Serialize(itemData)
                .Returns(jsonItemData);

            var actualItem = _factory.CreateItemFromRequest(ItemOwnerType.Character, 10, request);

            actualItem.ItemTemplateId.Should().Be(12);
            actualItem.Data.Should().BeEquivalentTo(jsonItemData);
        }

        [Test]
        public void CreateItemFromRequest_AndOwnerTypeIsCharacter_ShouldSetCharacterId()
        {
            const int characterId = 10;
            var request = new CreateItemRequest();

            var actualItem = _factory.CreateItemFromRequest(ItemOwnerType.Character, characterId, request);

            actualItem.CharacterId.Should().Be(characterId);
        }

        [Test]
        public void CreateItemFromRequest_AndOwnerTypeIsLoot_ShouldSetLootId()
        {
            const int lootId = 10;
            var request = new CreateItemRequest();

            var actualItem = _factory.CreateItemFromRequest(ItemOwnerType.Loot, lootId, request);

            actualItem.LootId.Should().Be(lootId);
        }

        [Test]
        public void CreateItemFromRequest_AndOwnerTypeIsMonster_ShouldSetMonsterId()
        {
            const int monsterId = 10;
            var request = new CreateItemRequest();

            var actualItem = _factory.CreateItemFromRequest(ItemOwnerType.Monster, monsterId, request);

            actualItem.MonsterId.Should().Be(monsterId);
        }


    }
}