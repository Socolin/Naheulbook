using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class ItemServiceTests
    {
        private FakeUnitOfWorkFactory _unitOfWorkFactory;
        private IItemFactory _itemFactory;
        private FakeNotificationSessionFactory _notificationSessionFactory;
        private IAuthorizationUtil _authorizationUtil;
        private IItemUtil _itemUtil;
        private IJsonUtil _jsonUtil;
        private ItemService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _itemFactory = Substitute.For<IItemFactory>();
            _notificationSessionFactory = new FakeNotificationSessionFactory();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _itemUtil = Substitute.For<IItemUtil>();
            _jsonUtil = Substitute.For<IJsonUtil>();

            _service = new ItemService(
                _unitOfWorkFactory,
                _itemFactory,
                _notificationSessionFactory,
                _authorizationUtil,
                _itemUtil,
                _jsonUtil
            );
        }

        [Test]
        public async Task AddItemToAsync_ShouldCreateAnItemInDatabase_ThenReturnsAFullyLoadedItem()
        {
            const int itemId = 25;
            const int characterId = 10;
            const int itemTemplateId = 12;
            var executionContext = new NaheulbookExecutionContext();
            var itemData = new ItemData();
            var itemTemplate = new ItemTemplate();
            var request = new CreateItemRequest {ItemData = itemData, ItemTemplateId = itemTemplateId};
            var createdItem = new Item {Id = itemId};
            var fullyLoadedItem = new Item();

            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetAsync(itemTemplateId)
                .Returns(itemTemplate);
            _itemFactory.CreateItemFromRequest(ItemOwnerType.Character, characterId, itemTemplate, itemData)
                .Returns(createdItem);
            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithAllDataAsync(itemId)
                .Returns(fullyLoadedItem);

            var actualItem = await _service.AddItemToAsync(executionContext, ItemOwnerType.Character, characterId, request);

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
            var itemData = JObject.FromObject(new {a = "b"});
            var item = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Items.GetWithOwnerAsync(itemId)
                .Returns(item);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.CompleteAsync())
                .Do(info => item.Data.Should().Be(itemData.ToString(Formatting.None)));

            var actualItem = await _service.UpdateItemDataAsync(new NaheulbookExecutionContext(), itemId, itemData);

            actualItem.Should().BeSameAs(item);
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

            Func<Task> act = () =>  _service.UpdateItemDataAsync(executionContext, itemId, new JObject());

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

            await _service.UpdateItemDataAsync(new NaheulbookExecutionContext(), itemId, new JObject());

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

            Func<Task> act = () =>  _service.UpdateItemModifiersAsync(executionContext, itemId, new List<ActiveStatsModifier>());

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

            Func<Task> act = () =>  _service.EquipItemAsync(executionContext, itemId, new EquipItemRequest());

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

            Func<Task> act = () =>  _service.ChangeItemContainerAsync(new NaheulbookExecutionContext(), 4, new ChangeItemContainerRequest());

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

            Func<Task> act = () =>  _service.ChangeItemContainerAsync(executionContext, itemId, new ChangeItemContainerRequest());

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
    }
}