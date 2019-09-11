using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.Exceptions;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class ItemServiceTests
    {
        private const string ItemDataJson = "some-item-data-json";
        private FakeUnitOfWorkFactory _unitOfWorkFactory;
        private IItemFactory _itemFactory;
        private FakeNotificationSessionFactory _notificationSessionFactory;
        private IAuthorizationUtil _authorizationUtil;
        private IItemUtil _itemUtil;
        private IJsonUtil _jsonUtil;
        private ICharacterHistoryUtil _characterHistoryUtil;
        private IRngUtil _rngUtil;
        private IItemTemplateUtil _itemTemplateUtil;

        private ItemService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _itemFactory = Substitute.For<IItemFactory>();
            _notificationSessionFactory = new FakeNotificationSessionFactory();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _itemUtil = Substitute.For<IItemUtil>();
            _characterHistoryUtil = Substitute.For<ICharacterHistoryUtil>();
            _jsonUtil = Substitute.For<IJsonUtil>();
            _itemTemplateUtil = Substitute.For<IItemTemplateUtil>();

            _rngUtil = Substitute.For<IRngUtil>();
            _service = new ItemService(
                _unitOfWorkFactory,
                _itemFactory,
                _notificationSessionFactory,
                _authorizationUtil,
                _itemUtil,
                _characterHistoryUtil,
                _jsonUtil,
                _rngUtil,
                _itemTemplateUtil
            );
        }

        [Test]
        public async Task AddItemToAsync_ShouldCreateAnItemInDatabase_ThenReturnsAFullyLoadedItem()
        {
            const int itemId = 25;
            const int characterId = 10;
            const int itemTemplateId = 12;
            var itemData = new ItemData();
            var itemTemplate = new ItemTemplate();
            var request = new CreateItemRequest {ItemData = itemData, ItemTemplateId = itemTemplateId};
            var createdItem = new Item {Id = itemId};
            var fullyLoadedItem = new Item();

            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetAsync(itemTemplateId)
                .Returns(itemTemplate);
            _itemFactory.CreateItem(ItemOwnerType.Character, characterId, itemTemplate, itemData)
                .Returns(createdItem);
            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithAllDataAsync(itemId)
                .Returns(fullyLoadedItem);

            var actualItem = await _service.AddItemToAsync(ItemOwnerType.Character, characterId, request);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().Items.Add(createdItem);
                _unitOfWorkFactory.GetUnitOfWork().CompleteAsync();
            });
            actualItem.Should().BeSameAs(fullyLoadedItem);
        }

        [Test]
        public async Task UpdateItemDataAsync_ShouldUpdateItemDataFieldAndSaveDb()
        {

            const int itemId = 4;
            var itemData = new ItemData();
            var item = GivenAnItem(new ItemData());

            _jsonUtil.Serialize(itemData)
                .Returns("some-new-item-data-json");
            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.CompleteAsync())
                .Do(info => item.Data.Should().Be("some-new-item-data-json"));

            var actualItem = await _service.UpdateItemDataAsync(new NaheulbookExecutionContext(), itemId, itemData);

            actualItem.Should().BeSameAs(item);
            await _unitOfWorkFactory.GetUnitOfWork().Received(1).CompleteAsync();
        }

        [Test]
        [TestCase(null, 2)]
        [TestCase(3, 4)]
        [TestCase(8, 2)]
        [TestCase(8, null)]
        public async Task UpdateItemDataAsync_WhenQuantityChange_ShouldLogItInCharacterHistory(int? currentQuantity, int? newQuantity)
        {
            const int itemId = 4;
            const int characterId = 8;
            var itemData = new ItemData {Quantity = newQuantity};
            var item = GivenAnItem(new ItemData {Quantity = currentQuantity}, characterId);
            var characterHistoryEntry = new CharacterHistoryEntry();

            _jsonUtil.Serialize(itemData)
                .Returns("some-new-item-data-json");
            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);
            _characterHistoryUtil.CreateLogChangeItemQuantity(characterId, item, currentQuantity, newQuantity)
                .Returns(characterHistoryEntry);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.CompleteAsync())
                .Do(info => item.Character.HistoryEntries.Should().Contain(characterHistoryEntry));

            await _service.UpdateItemDataAsync(new NaheulbookExecutionContext(), itemId, itemData);

            await _unitOfWorkFactory.GetUnitOfWork().Received(1).CompleteAsync();
        }

        [Test]
        public async Task UpdateItemDataAsync_WhenChargeDecrementByOne_ShouldLogItInCharacterHistory()
        {
            const int itemId = 4;
            const int characterId = 8;
            var itemData = new ItemData {Charge = 3};
            var item = GivenAnItem(new ItemData {Charge = 4}, characterId);
            var characterHistoryEntry = new CharacterHistoryEntry();

            _jsonUtil.Serialize(itemData)
                .Returns("some-new-item-data-json");
            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);
            _characterHistoryUtil.CreateLogUseItemCharge(characterId, item, 4, 3)
                .Returns(characterHistoryEntry);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.CompleteAsync())
                .Do(info => item.Character.HistoryEntries.Should().Contain(characterHistoryEntry));

            await _service.UpdateItemDataAsync(new NaheulbookExecutionContext(), itemId, itemData);

            await _unitOfWorkFactory.GetUnitOfWork().Received(1).CompleteAsync();
        }

        [Test]
        public void UpdateItemDataAsync_EnsureCurrentUserCanAccessThisItem()
        {
            const int itemId = 4;
            var executionContext = new NaheulbookExecutionContext();
            var item = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);
            _authorizationUtil.When(x => x.EnsureItemAccess(executionContext, item))
                .Throw(new TestException());

            Func<Task> act = () => _service.UpdateItemDataAsync(executionContext, itemId, new ItemData());

            act.Should().Throw<TestException>();
            _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().CompleteAsync();
        }

        [Test]
        public async Task UpdateItemDataAsync_ShouldCallChangeNotifier()
        {
            const int itemId = 4;
            var item = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);

            await _service.UpdateItemDataAsync(new NaheulbookExecutionContext(), itemId, new ItemData());

            Received.InOrder(() =>
            {
                _notificationSessionFactory.NotificationSession.NotifyItemDataChanged(item);
                _notificationSessionFactory.NotificationSession.CommitAsync();
            });
        }

        [Test]
        public async Task UpdateItemModifiersAsync_ShouldUpdateItemModifiersFieldAndSaveDb()
        {
            const int itemId = 4;
            var itemModifiers = new List<ActiveStatsModifier>();
            var itemModifiersJson = "some-json";
            var item = new Item();

            _jsonUtil.Serialize(itemModifiers)
                .Returns(itemModifiersJson);
            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.CompleteAsync())
                .Do(info => item.Modifiers.Should().Be(itemModifiersJson));

            var actualItem = await _service.UpdateItemModifiersAsync(new NaheulbookExecutionContext(), itemId, itemModifiers);

            actualItem.Should().BeSameAs(item);
            await _unitOfWorkFactory.GetUnitOfWork().Received(1).CompleteAsync();
        }

        [Test]
        public void UpdateItemModifiersAsync_EnsureCurrentUserCanAccessThisItem()
        {
            const int itemId = 4;
            var executionContext = new NaheulbookExecutionContext();
            var item = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);
            _authorizationUtil.When(x => x.EnsureItemAccess(executionContext, item))
                .Throw(new TestException());

            Func<Task> act = () => _service.UpdateItemModifiersAsync(executionContext, itemId, new List<ActiveStatsModifier>());

            act.Should().Throw<TestException>();
            _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().CompleteAsync();
        }

        [Test]
        public async Task UpdateItemModifiersAsync_ShouldCallChangeNotifier()
        {
            const int itemId = 4;
            var item = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);

            await _service.UpdateItemModifiersAsync(new NaheulbookExecutionContext(), itemId, new List<ActiveStatsModifier>());

            Received.InOrder(() =>
            {
                _notificationSessionFactory.NotificationSession.NotifyItemModifiersChanged(item);
                _notificationSessionFactory.NotificationSession.CommitAsync();
            });
        }

        [Test]
        public async Task EquipItemAsync_ShouldUpdateItemModifiersFieldAndSaveDb()
        {
            const int itemId = 4;
            var equipRequest = new EquipItemRequest {Level = 24};
            var item = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.CompleteAsync())
                .Do(info => _itemUtil.Received(1).EquipItem(item, 24));

            var actualItem = await _service.EquipItemAsync(new NaheulbookExecutionContext(), itemId, equipRequest);

            actualItem.Should().BeSameAs(item);
            await _unitOfWorkFactory.GetUnitOfWork().Received(1).CompleteAsync();
        }

        [Test]
        public void EquipItemAsync_EnsureCurrentUserCanAccessThisItem()
        {
            const int itemId = 4;
            var executionContext = new NaheulbookExecutionContext();
            var item = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);
            _authorizationUtil.When(x => x.EnsureItemAccess(executionContext, item))
                .Throw(new TestException());

            Func<Task> act = () => _service.EquipItemAsync(executionContext, itemId, new EquipItemRequest());

            act.Should().Throw<TestException>();
            _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().CompleteAsync();
        }

        [Test]
        public async Task EquipItemAsync_ShouldCallChangeNotifier()
        {
            const int itemId = 4;
            var item = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);

            await _service.EquipItemAsync(new NaheulbookExecutionContext(), itemId, new EquipItemRequest());

            Received.InOrder(() =>
            {
                _notificationSessionFactory.NotificationSession.NotifyEquipItem(item);
                _notificationSessionFactory.NotificationSession.CommitAsync();
            });
        }


        [Test]
        public async Task ChangeItemContainerAsync_ShouldChangeItemContainerAndSaveItInDb()
        {
            const int itemId = 4;
            const int containerId = 8;
            var equipRequest = new ChangeItemContainerRequest {ContainerId = containerId};
            var item = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.CompleteAsync())
                .Do(info => item.ContainerId.Should().Be(containerId));

            var actualItem = await _service.ChangeItemContainerAsync(new NaheulbookExecutionContext(), itemId, equipRequest);

            actualItem.Should().BeSameAs(item);
            await _unitOfWorkFactory.GetUnitOfWork().Received(1).CompleteAsync();
        }

        [Test]
        public void ChangeItemContainerAsync_ShouldThrowIfItemIsNotFound()
        {
            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(Arg.Any<int>())
                .Returns((Item) null);

            Func<Task> act = () => _service.ChangeItemContainerAsync(new NaheulbookExecutionContext(), 4, new ChangeItemContainerRequest());

            act.Should().Throw<ItemNotFoundException>();
        }

        [Test]
        public void ChangeItemContainerAsync_EnsureCurrentUserCanAccessThisItem()
        {
            const int itemId = 4;
            var executionContext = new NaheulbookExecutionContext();
            var item = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);
            _authorizationUtil.When(x => x.EnsureItemAccess(executionContext, item))
                .Throw(new TestException());

            Func<Task> act = () => _service.ChangeItemContainerAsync(executionContext, itemId, new ChangeItemContainerRequest());

            act.Should().Throw<TestException>();
            _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().CompleteAsync();
        }

        [Test]
        public async Task ChangeItemContainerAsync_ShouldCallChangeNotifier()
        {
            const int itemId = 4;
            var item = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);

            await _service.ChangeItemContainerAsync(new NaheulbookExecutionContext(), itemId, new ChangeItemContainerRequest());

            Received.InOrder(() =>
            {
                _notificationSessionFactory.NotificationSession.NotifyItemChangeContainer(item);
                _notificationSessionFactory.NotificationSession.CommitAsync();
            });
        }

        [Test]
        public async Task CreateItemsAsync_ShouldReturnsAnEmptyList_WhenRequestIsNull()
        {
            var result = await _service.CreateItemsAsync(null);
            result.Should().BeEmpty();
        }

        [Test]
        public async Task CreateItemsAsync_ShouldReturnsAnEmptyList_WhenRequestIsEmpty()
        {
            var result = await _service.CreateItemsAsync(new List<CreateItemRequest>());
            result.Should().BeEmpty();
        }

        [Test]
        public void CreateItemsAsync_ShouldThrowIfItemTemplateIsNotFound()
        {
            var createItemRequests = new List<CreateItemRequest>
            {
                new CreateItemRequest {ItemTemplateId = 42},
            };

            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetByIdsAsync(Arg.Any<IEnumerable<int>>())
                .Returns(new List<ItemTemplate>());

            Func<Task> act = () => _service.CreateItemsAsync(createItemRequests);

            act.Should().Throw<ItemTemplateNotFoundException>();
        }

        [Test]
        public async Task CreateItemsAsync_ShouldCreateNewItems_ForEachElementOfTheRequest()
        {
            const int itemTemplateId1 = 1;
            const int itemTemplateId2 = 2;

            var itemData1 = new ItemData();
            var itemData2 = new ItemData();
            var itemTemplate1 = new ItemTemplate {Id = itemTemplateId1};
            var itemTemplate2 = new ItemTemplate {Id = itemTemplateId2};
            var item1 = new Item();
            var item2 = new Item();

            var createItemRequests = new List<CreateItemRequest>
            {
                new CreateItemRequest {ItemTemplateId = itemTemplateId1, ItemData = itemData1},
                new CreateItemRequest {ItemTemplateId = itemTemplateId2, ItemData = itemData2},
            };

            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetByIdsAsync(Arg.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] {itemTemplateId1, itemTemplateId2})))
                .Returns(new List<ItemTemplate> {itemTemplate1, itemTemplate2});
            _itemFactory.CreateItem(itemTemplate1, itemData1)
                .Returns(item1);
            _itemFactory.CreateItem(itemTemplate2, itemData2)
                .Returns(item2);

            var result = await _service.CreateItemsAsync(createItemRequests);

            result.ElementAt(0).Should().BeSameAs(item1);
            result.ElementAt(1).Should().BeSameAs(item2);
        }

        private Item GivenAnItem(ItemData itemData = null, int? characterId = null)
        {
            _jsonUtil.Deserialize<ItemData>(ItemDataJson)
                .Returns(itemData ?? new ItemData());

            return new Item
            {
                CharacterId = characterId,
                Character = !characterId.HasValue
                    ? null
                    : new Character
                    {
                        Id = characterId.Value
                    },
                Data = ItemDataJson
            };
        }
    }
}