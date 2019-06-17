using System;
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
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class CharacterServiceTests
    {
        private FakeUnitOfWorkFactory _unitOfWorkFactory;
        private ICharacterFactory _characterFactory;
        private IItemService _itemService;
        private IAuthorizationUtil _authorizationUtil;
        private CharacterService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _characterFactory = Substitute.For<ICharacterFactory>();
            _itemService = Substitute.For<IItemService>();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();

            _service = new CharacterService(_unitOfWorkFactory, _characterFactory, _itemService, _authorizationUtil);
        }

        [Test]
        public async Task CreateCharacterAsync_CreateANewCharacterInDatabase_SettingOwnerToCurrentUserId()
        {
            const int userId = 10;
            var createCharacterRequest = new CreateCharacterRequest {Name = "some-name"};
            var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = userId};
            var createdCharacter = new Character();

            _characterFactory.CreateCharacter(createCharacterRequest)
                .Returns(createdCharacter);

            var actualCharacter = await _service.CreateCharacterAsync(naheulbookExecutionContext, createCharacterRequest);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().Characters.Add(createdCharacter);
                _unitOfWorkFactory.GetUnitOfWork().CompleteAsync();
            });

            actualCharacter.OwnerId.Should().Be(userId);
            createdCharacter.Should().BeSameAs(actualCharacter);
        }

        [Test]
        public void LoadCharacterDetailsAsync_ShouldCall_EnsureCharacterAccess()
        {
            const int characterId = 4;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithAllDataAsync(characterId)
                .Returns(character);
            _authorizationUtil.When(x => x.EnsureCharacterAccess(executionContext, character))
                .Throw(new TestException());

            Func<Task> act = () => _service.LoadCharacterDetailsAsync(executionContext, characterId);

            act.Should().Throw<TestException>();
        }

        [Test]
        public async Task AddItemToCharacterAsync_ShouldCall_ItemService()
        {
            const int characterId = 4;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();
            var request = new CreateItemRequest();
            var expectedItem = new Item();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _itemService.AddItemToAsync(executionContext, ItemOwnerType.Character, characterId, request)
                .Returns(expectedItem);

            var item = await _service.AddItemToCharacterAsync(executionContext, characterId, request);

            item.Should().BeSameAs(expectedItem);
        }

        [Test]
        public void AddItemToCharacterAsync_ShouldCall_EnsureCharacterAccess()
        {
            const int characterId = 4;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();
            var request = new CreateItemRequest();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _authorizationUtil.When(x => x.EnsureCharacterAccess(executionContext, character))
                .Throw(new TestException());

            Func<Task> act = () => _service.AddItemToCharacterAsync(executionContext, characterId, request);

            act.Should().Throw<TestException>();
        }
    }
}