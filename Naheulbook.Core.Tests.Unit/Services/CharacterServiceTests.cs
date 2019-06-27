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
        private ICharacterHistoryUtil _characterHistoryUtil;
        private IChangeNotifier _changeNotifier;
        private CharacterService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _characterFactory = Substitute.For<ICharacterFactory>();
            _itemService = Substitute.For<IItemService>();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _characterHistoryUtil = Substitute.For<ICharacterHistoryUtil>();
            _changeNotifier = Substitute.For<IChangeNotifier>();

            _service = new CharacterService(
                _unitOfWorkFactory,
                _characterFactory,
                _itemService,
                _authorizationUtil,
                _characterHistoryUtil,
                _changeNotifier
            );
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
        public async Task AddItemToCharacterAsync_ShouldNotifyChanges()
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

            await _service.AddItemToCharacterAsync(executionContext, characterId, request);

            await _changeNotifier.Received(1).NotifyCharacterAddItemAsync(characterId, expectedItem);
        }

        [Test]
        public async Task AddItemToCharacterAsync_ShouldAddACharacterHistoryEntry()
        {
            const int characterId = 4;
            var item = new Item();
            var expectedCharacterHistoryEntry = new CharacterHistoryEntry();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(Arg.Any<int>())
                .Returns(new Character {Id = characterId});
            _itemService.AddItemToAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<ItemOwnerType>(), Arg.Any<int>(), Arg.Any<CreateItemRequest>())
                .Returns(item);
            _characterHistoryUtil.CreateLogAddItem(characterId, item)
                .Returns(expectedCharacterHistoryEntry);

            await _service.AddItemToCharacterAsync(new NaheulbookExecutionContext(), characterId, new CreateItemRequest());

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork(1).CharacterHistoryEntries.Add(expectedCharacterHistoryEntry);
                _unitOfWorkFactory.GetUnitOfWork(1).CompleteAsync();
            });
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

        [Test]
        public async Task GetCharacterLootsAsync_ShouldLoadDataFromLootRepository()
        {
            const int characterId = 4;
            const int groupId = 6;
            var character = new Character {Id = characterId, GroupId = groupId};
            var executionContext = new NaheulbookExecutionContext();
            var expectedLoots = new List<Loot>();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _unitOfWorkFactory.GetUnitOfWork().Loots.GetLootsVisibleByCharactersOfGroupAsync(groupId)
                .Returns(expectedLoots);

            var loots = await _service.GetCharacterLootsAsync(executionContext, characterId);

            loots.Should().BeSameAs(expectedLoots);
        }

        [Test]
        public void GetCharacterLootsAsync_ShouldCall_EnsureCharacterAccess()
        {
            const int characterId = 4;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _authorizationUtil.When(x => x.EnsureCharacterAccess(executionContext, character))
                .Throw(new TestException());

            Func<Task> act = () => _service.GetCharacterLootsAsync(executionContext, characterId);

            act.Should().Throw<TestException>();
        }

        [Test]
        [TestCase(3, false)]
        [TestCase(4, true)]
        public async Task GetCharacterHistoryEntryAsync_ShouldGetResultFromCharacterRepository(int masterId, bool isGm)
        {
            const int characterId = 4;
            const int groupId = 6;
            var character = new Character {Id = characterId, GroupId = groupId, Group = new Group {Id = groupId, MasterId = masterId}};
            var executionContext = new NaheulbookExecutionContext();
            var expectedHistoryEntries = new List<IHistoryEntry>();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _unitOfWorkFactory.GetUnitOfWork().Characters.GetHistoryByCharacterIdAsync(characterId, groupId, 0, isGm)
                .Returns(expectedHistoryEntries);

            var loots = await _service.GetCharacterHistoryEntryAsync(executionContext, characterId, 0);

            loots.Should().BeSameAs(expectedHistoryEntries);
        }

        [Test]
        public void GetCharacterHistoryEntryAsync_ShouldCall_EnsureCharacterAccess()
        {
            const int characterId = 4;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _authorizationUtil.When(x => x.EnsureCharacterAccess(executionContext, character))
                .Throw(new TestException());

            Func<Task> act = () => _service.GetCharacterHistoryEntryAsync(executionContext, characterId, 0);

            act.Should().Throw<TestException>();
        }

        [Test]
        public void GetCharacterHistoryEntryAsync_ShouldThrowWhenCharacterDoesNotExists()
        {
            const int characterId = 4;
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns((Character) null);

            Func<Task> act = () => _service.GetCharacterHistoryEntryAsync(executionContext, characterId, 0);

            act.Should().Throw<CharacterNotFoundException>();
        }

        [Test]
        public void UpdateCharacterStatAsync_ShouldCall_EnsureCharacterAccess()
        {
            const int characterId = 4;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _authorizationUtil.When(x => x.EnsureCharacterAccess(executionContext, character))
                .Throw(new TestException());

            Func<Task> act = () => _service.UpdateCharacterStatAsync(executionContext, characterId, new PatchCharacterStatsRequest());

            act.Should().Throw<TestException>();
        }

        [Test]
        public void UpdateCharacterStatAsync_ShouldThrowWhenCharacterDoesNotExists()
        {
            const int characterId = 4;
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns((Character) null);

            Func<Task> act = () => _service.UpdateCharacterStatAsync(executionContext, characterId, new PatchCharacterStatsRequest());

            act.Should().Throw<CharacterNotFoundException>();
        }

        [Test]
        public async Task SetCharacterAdBonusStatAsync_ShouldChangeStatValueAndSave()
        {
            const int characterId = 4;
            var executionContext = new NaheulbookExecutionContext();
            var character = new Character();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.CompleteAsync())
                .Do(info => character.StatBonusAd.Should().BeEquivalentTo("some-stat"));

            await _service.SetCharacterAdBonusStatAsync(executionContext, characterId, new PutStatBonusAdRequest {Stat = "some-stat"});

            await _unitOfWorkFactory.GetUnitOfWork().Received(1).CompleteAsync();
            await _changeNotifier.Received(1).NotifyCharacterSetStatBonusAdAsync(character.Id, "some-stat");
        }

        [Test]
        public void SetCharacterAdBonusStatAsync_ShouldCall_EnsureCharacterAccess()
        {
            const int characterId = 4;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _authorizationUtil.When(x => x.EnsureCharacterAccess(executionContext, character))
                .Throw(new TestException());

            Func<Task> act = () => _service.SetCharacterAdBonusStatAsync(executionContext, characterId, new PutStatBonusAdRequest());

            act.Should().Throw<TestException>();
        }

        [Test]
        public void SetCharacterAdBonusStatAsync_ShouldThrowWhenCharacterDoesNotExists()
        {
            const int characterId = 4;
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns((Character) null);

            Func<Task> act = () => _service.SetCharacterAdBonusStatAsync(executionContext, characterId, new PutStatBonusAdRequest());

            act.Should().Throw<CharacterNotFoundException>();
        }
    }
}