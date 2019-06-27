using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
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
        private IChangeNotifier _changeNotifier;
        private IAuthorizationUtil _authorizationUtil;
        private IJsonUtil _jsonUtil;
        private ItemService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _itemFactory = Substitute.For<IItemFactory>();
            _changeNotifier = Substitute.For<IChangeNotifier>();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _jsonUtil = Substitute.For<IJsonUtil>();

            _service = new ItemService(
                _unitOfWorkFactory,
                _itemFactory,
                _changeNotifier,
                _authorizationUtil,
                _jsonUtil
            );
        }

        [Test]
        public async Task AddItemToAsync_ShouldCreateAnItemInDatabase_ThenReturnsAFullyLoadedItem()
        {
            const int itemId = 25;
            const int characterId = 10;
            var executionContext = new NaheulbookExecutionContext();
            var request = new CreateItemRequest();
            var createdItem = new Item {Id = itemId};
            var fullyLoadedItem = new Item();

            _itemFactory.CreateItemFromRequest(ItemOwnerType.Character, characterId, request)
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

            await _changeNotifier.Received(1).NotifyItemDataChangedAsync(item);
        }

        [Test]
        public async Task UpdateItemModifiersAsync_ShouldUpdateItemModifiersFieldAndSaveDb()
        {
            const int itemId = 4;
            var itemModifiers = new List<ActiveStatsModifier>();
            var itemModifiersJson = "some-json";
            var item = new Item();;

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

            await _changeNotifier.Received(1).NotifyItemModifiersChangedAsync(item);
        }
    }
}