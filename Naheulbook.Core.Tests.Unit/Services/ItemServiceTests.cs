using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class ItemServiceTests
    {
        private FakeUnitOfWorkFactory _unitOfWorkFactory;
        private ItemService _service;
        private IItemFactory _itemFactory;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _itemFactory = Substitute.For<IItemFactory>();

            _service = new ItemService(_unitOfWorkFactory, _itemFactory);
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
    }
}