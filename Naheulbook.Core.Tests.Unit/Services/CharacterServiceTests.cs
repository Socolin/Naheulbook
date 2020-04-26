using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private IMapper _mapper;
        private ICharacterModifierUtil _characterModifierUtil;
        private FakeNotificationSessionFactory _notificationSessionFactory;
        private ICharacterUtil _characterUtil;
        private IItemUtil _itemUtil;
        private CharacterService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _characterFactory = Substitute.For<ICharacterFactory>();
            _itemService = Substitute.For<IItemService>();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _characterHistoryUtil = Substitute.For<ICharacterHistoryUtil>();
            _mapper = Substitute.For<IMapper>();
            _characterModifierUtil = Substitute.For<ICharacterModifierUtil>();
            _notificationSessionFactory = new FakeNotificationSessionFactory();
            _characterUtil = Substitute.For<ICharacterUtil>();
            _itemUtil = Substitute.For<IItemUtil>();

            _service = new CharacterService(
                _unitOfWorkFactory,
                _characterFactory,
                _itemService,
                _authorizationUtil,
                _characterHistoryUtil,
                _mapper,
                _characterModifierUtil,
                _notificationSessionFactory,
                _characterUtil,
                _itemUtil
            );
        }

        [Test]
        public async Task CreateCharacterAsync_CreateANewCharacterInDatabase_SettingOwnerToCurrentUserId()
        {
            const int userId = 10;
            var createCharacterRequest = new CreateCharacterRequest {Name = "some-name"};
            var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = userId};
            var createdCharacter = new Character();
            var initialInventory = new List<Item>();

            _characterFactory.CreateCharacter(createCharacterRequest)
                .Returns(createdCharacter);
            _itemUtil.CreateInitialPlayerInventoryAsync(createCharacterRequest.Money)
                .Returns(initialInventory);

            var actualCharacter = await _service.CreateCharacterAsync(naheulbookExecutionContext, createCharacterRequest);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().Characters.Add(createdCharacter);
                _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
            });

            actualCharacter.OwnerId.Should().Be(userId);
            createdCharacter.Should().BeSameAs(actualCharacter);
            createdCharacter.Items.Should().BeSameAs(initialInventory);
        }

        [Test]
        public async Task CreateCharacterAsync_WhenGroupIdIsGiven_PutCharacterInGroup()
        {
            const int userId = 10;
            const int groupId = 8;
            var createCharacterRequest = new CreateCharacterRequest {Name = "some-name", GroupId = groupId};
            var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = userId};
            var createdCharacter = new Character();
            var group = new Group();

            _characterFactory.CreateCharacter(createCharacterRequest)
                .Returns(createdCharacter);
            _itemUtil.CreateInitialPlayerInventoryAsync(createCharacterRequest.Money)
                .Returns(new List<Item>());
            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);

            var actualCharacter = await _service.CreateCharacterAsync(naheulbookExecutionContext, createCharacterRequest);

            actualCharacter.Group.Should().BeSameAs(group);
        }

        [Test]
        public void CreateCharacterAsync_WhenGroupIdIsGiven_CheckIsGroupOwner()
        {
            const int groupId = 8;
            var createCharacterRequest = new CreateCharacterRequest {Name = "some-name", GroupId = groupId};
            var naheulbookExecutionContext = new NaheulbookExecutionContext();
            var createdCharacter = new Character();
            var group = new Group();

            _characterFactory.CreateCharacter(createCharacterRequest)
                .Returns(createdCharacter);
            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);
            _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
                .Throw(new ForbiddenAccessException());

            Func<Task> act = () => _service.CreateCharacterAsync(naheulbookExecutionContext, createCharacterRequest);

            act.Should().Throw<ForbiddenAccessException>();
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
            _itemService.AddItemToAsync(ItemOwnerType.Character, characterId, request)
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
            _itemService.AddItemToAsync(ItemOwnerType.Character, characterId, request)
                .Returns(expectedItem);

            await _service.AddItemToCharacterAsync(executionContext, characterId, request);

            Received.InOrder(() =>
            {
                _notificationSessionFactory.NotificationSession.NotifyCharacterAddItem(characterId, expectedItem);
                _notificationSessionFactory.NotificationSession.CommitAsync();
            });
        }

        [Test]
        public async Task AddItemToCharacterAsync_ShouldAddACharacterHistoryEntry()
        {
            const int characterId = 4;
            var item = new Item();
            var expectedCharacterHistoryEntry = new CharacterHistoryEntry();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(Arg.Any<int>())
                .Returns(new Character {Id = characterId});
            _itemService.AddItemToAsync(Arg.Any<ItemOwnerType>(), Arg.Any<int>(), Arg.Any<CreateItemRequest>())
                .Returns(item);
            _characterHistoryUtil.CreateLogAddItem(characterId, item)
                .Returns(expectedCharacterHistoryEntry);

            await _service.AddItemToCharacterAsync(new NaheulbookExecutionContext(), characterId, new CreateItemRequest());

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork(1).CharacterHistoryEntries.Add(expectedCharacterHistoryEntry);
                _unitOfWorkFactory.GetUnitOfWork(1).SaveChangesAsync();
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

            Func<Task> act = () => _service.UpdateCharacterAsync(executionContext, characterId, new PatchCharacterRequest());

            act.Should().Throw<TestException>();
        }

        [Test]
        public void UpdateCharacterStatAsync_ShouldThrowWhenCharacterDoesNotExists()
        {
            const int characterId = 4;
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns((Character) null);

            Func<Task> act = () => _service.UpdateCharacterAsync(executionContext, characterId, new PatchCharacterRequest());

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
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.SaveChangesAsync())
                .Do(info => character.StatBonusAd.Should().BeEquivalentTo("some-stat"));

            await _service.SetCharacterAdBonusStatAsync(executionContext, characterId, new PutStatBonusAdRequest {Stat = "some-stat"});

            await _unitOfWorkFactory.GetUnitOfWork().Received(1).SaveChangesAsync();

            Received.InOrder(() =>
            {
                _notificationSessionFactory.NotificationSession.NotifyCharacterSetStatBonusAd(characterId, "some-stat");
                _notificationSessionFactory.NotificationSession.CommitAsync();
            });
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

        [Test]
        public async Task AddModifiersAsync_ShouldAddCharacterModifier()
        {
            const int characterId = 4;
            var executionContext = new NaheulbookExecutionContext();
            var character = new Character();
            var request = new AddCharacterModifierRequest();
            var characterModifier = new CharacterModifier();

            _mapper.Map<CharacterModifier>(request)
                .Returns(characterModifier);
            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);

            var actualCharacterModifier = await _service.AddModifiersAsync(executionContext, characterId, request);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().CharacterModifiers.Add(characterModifier);
                _unitOfWorkFactory.GetUnitOfWork().Received(1).SaveChangesAsync();
            });
            actualCharacterModifier.Should().BeSameAs(characterModifier);
        }

        [Test]
        public async Task AddModifiersAsync_ShouldNotifyChange()
        {
            const int characterId = 4;
            var character = new Character();
            var characterModifier = new CharacterModifier();

            _mapper.Map<CharacterModifier>(Arg.Any<AddCharacterModifierRequest>())
                .Returns(characterModifier);
            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);

            await _service.AddModifiersAsync(new NaheulbookExecutionContext(), characterId, new AddCharacterModifierRequest());

            Received.InOrder(() =>
            {
                _notificationSessionFactory.NotificationSession.NotifyCharacterAddModifier(characterId, characterModifier);
                _notificationSessionFactory.NotificationSession.CommitAsync();
            });
        }

        [Test]
        public async Task AddModifiersAsync_ShouldLog()
        {
            const int characterId = 4;
            var character = new Character();
            var characterHistoryEntry = new CharacterHistoryEntry();
            var request = new AddCharacterModifierRequest();
            var characterModifier = new CharacterModifier();

            _mapper.Map<CharacterModifier>(request)
                .Returns(characterModifier);

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _characterHistoryUtil.CreateLogAddModifier(character, characterModifier)
                .Returns(characterHistoryEntry);

            await _service.AddModifiersAsync(new NaheulbookExecutionContext(), characterId, request);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().CharacterHistoryEntries.Add(characterHistoryEntry);
                _unitOfWorkFactory.GetUnitOfWork().Received(1).SaveChangesAsync();
            });
        }

        [Test]
        public void AddModifiersAsync_ShouldCall_EnsureCharacterAccess()
        {
            const int characterId = 4;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _authorizationUtil.When(x => x.EnsureCharacterAccess(executionContext, character))
                .Throw(new TestException());

            Func<Task> act = () => _service.AddModifiersAsync(executionContext, characterId, new AddCharacterModifierRequest());

            act.Should().Throw<TestException>();
        }

        [Test]
        public void AddModifiersAsync_ShouldThrowWhenCharacterDoesNotExists()
        {
            const int characterId = 4;
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns((Character) null);

            Func<Task> act = () => _service.AddModifiersAsync(executionContext, characterId, new AddCharacterModifierRequest());

            act.Should().Throw<CharacterNotFoundException>();
        }

        [Test]
        public async Task DeleteModifiersAsync_ShouldDeleteCharacterModifier()
        {
            const int characterId = 4;
            const int characterModifierId = 2;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();
            var characterModifier = new CharacterModifier {Character = character, CharacterId = characterId};

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _unitOfWorkFactory.GetUnitOfWork().CharacterModifiers.GetByIdAndCharacterIdAsync(characterId, characterModifierId)
                .Returns(characterModifier);

            await _service.DeleteModifiersAsync(executionContext, characterId, characterModifierId);

            Received.InOrder(() =>
            {
                // TODO: Change after rework of character history
                characterModifier.CharacterId.Should().BeNull();
                // _unitOfWorkFactory.GetUnitOfWork().CharacterModifiers.Remove(characterModifier);
                _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
            });
        }

        [Test]
        public async Task DeleteModifiersAsync_ShouldLogCharacterHistory()
        {
            const int characterId = 4;
            const int characterModifierId = 2;
            var characterModifier = new CharacterModifier();
            var characterHistoryEntry = new CharacterHistoryEntry();

            _characterHistoryUtil.CreateLogRemoveModifier(characterId, characterModifier)
                .Returns(characterHistoryEntry);
            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(Arg.Any<int>())
                .Returns(new Character());
            _unitOfWorkFactory.GetUnitOfWork().CharacterModifiers.GetByIdAndCharacterIdAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns(characterModifier);

            await _service.DeleteModifiersAsync(new NaheulbookExecutionContext(), characterId, characterModifierId);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().CharacterHistoryEntries.Add(characterHistoryEntry);
                _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
            });
        }

        [Test]
        public async Task DeleteModifiersAsync_ShouldNotifyChange()
        {
            const int characterId = 4;
            const int characterModifierId = 2;

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(Arg.Any<int>())
                .Returns(new Character());
            _unitOfWorkFactory.GetUnitOfWork().CharacterModifiers.GetByIdAndCharacterIdAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns(new CharacterModifier());

            await _service.DeleteModifiersAsync(new NaheulbookExecutionContext(), characterId, characterModifierId);

            Received.InOrder(() =>
            {
                _notificationSessionFactory.NotificationSession.NotifyCharacterRemoveModifier(characterId, characterModifierId);
                _notificationSessionFactory.NotificationSession.CommitAsync();
            });
        }

        [Test]
        public void DeleteModifiersAsync_ShouldCall_EnsureCharacterAccess()
        {
            const int characterId = 4;
            const int characterModifierId = 2;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();
            var characterModifier = new CharacterModifier();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _unitOfWorkFactory.GetUnitOfWork().CharacterModifiers.GetAsync(characterId)
                .Returns(characterModifier);
            _authorizationUtil.When(x => x.EnsureCharacterAccess(executionContext, character))
                .Throw(new TestException());

            Func<Task> act = () => _service.DeleteModifiersAsync(executionContext, characterId, characterModifierId);

            act.Should().Throw<TestException>();
        }

        [Test]
        public void DeleteModifiersAsync_ShouldThrowWhenCharacterDoesNotExists()
        {
            const int characterId = 4;
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns((Character) null);

            Func<Task> act = () => _service.DeleteModifiersAsync(executionContext, characterId, 2);

            act.Should().Throw<CharacterNotFoundException>();
        }


        [Test]
        public async Task ToggleModifiersAsync_ShouldToggleModifier()
        {
            const int characterId = 4;
            const int characterModifierId = 2;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();
            var characterModifier = new CharacterModifier {Character = character, CharacterId = characterId};

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _unitOfWorkFactory.GetUnitOfWork().CharacterModifiers.GetByIdAndCharacterIdAsync(characterId, characterModifierId)
                .Returns(characterModifier);

            await _service.ToggleModifiersAsync(executionContext, characterId, characterModifierId);

            Received.InOrder(() =>
            {
                _characterModifierUtil.ToggleModifier(character, characterModifier);
                _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
            });
        }

        [Test]
        public async Task ToggleModifiersAsync_ShouldNotifyChange()
        {
            const int characterId = 4;
            const int characterModifierId = 2;
            var characterModifier = new CharacterModifier();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(Arg.Any<int>())
                .Returns(new Character());
            _unitOfWorkFactory.GetUnitOfWork().CharacterModifiers.GetByIdAndCharacterIdAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns(characterModifier);

            await _service.ToggleModifiersAsync(new NaheulbookExecutionContext(), characterId, characterModifierId);

            Received.InOrder(() =>
            {
                _notificationSessionFactory.NotificationSession.NotifyCharacterUpdateModifier(characterId, characterModifier);
                _notificationSessionFactory.NotificationSession.CommitAsync();
            });
        }

        [Test]
        public void ToggleModifiersAsync_ShouldCall_EnsureCharacterAccess()
        {
            const int characterId = 4;
            const int characterModifierId = 2;
            var character = new Character {Id = characterId};
            var executionContext = new NaheulbookExecutionContext();
            var characterModifier = new CharacterModifier();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns(character);
            _unitOfWorkFactory.GetUnitOfWork().CharacterModifiers.GetAsync(characterId)
                .Returns(characterModifier);
            _authorizationUtil.When(x => x.EnsureCharacterAccess(executionContext, character))
                .Throw(new TestException());

            Func<Task> act = () => _service.ToggleModifiersAsync(executionContext, characterId, characterModifierId);

            act.Should().Throw<TestException>();
        }

        [Test]
        public void ToggleModifiersAsync_ShouldThrowWhenCharacterDoesNotExists()
        {
            const int characterId = 4;
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithGroupAsync(characterId)
                .Returns((Character) null);

            Func<Task> act = () => _service.ToggleModifiersAsync(executionContext, characterId, 2);

            act.Should().Throw<CharacterNotFoundException>();
        }

        [Test]
        public void AddJobAsync_AddNewJobToCharacterJobsAndNotifyIt()
        {
            const int characterId = 5;
            var jobId = Guid.NewGuid();
            var executionContext = new NaheulbookExecutionContext();
            var request = new CharacterAddJobRequest {JobId = jobId};
            var character = new Character {Jobs = new List<CharacterJob>(), Id = characterId};
            var job = new Job {Id = jobId};

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
                .Returns(character);
            _unitOfWorkFactory.GetUnitOfWork().Jobs.GetAsync(jobId)
                .Returns(job);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.SaveChangesAsync())
                .Do(callInfo =>
                {
                    character.Jobs.Should().HaveCount(1);
                    character.Jobs.First().Job.Should().BeSameAs(job);
                });

            _service.AddJobAsync(executionContext, characterId, request);

            Received.InOrder(() =>
            {
                _notificationSessionFactory.NotificationSession.NotifyCharacterAddJob(characterId, jobId);
                _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
                _notificationSessionFactory.NotificationSession.Received().CommitAsync();
            });
        }

        [Test]
        public void AddJobAsync_WhenCharacterNotFound_Throw()
        {
            var jobId = Guid.NewGuid();
            const int characterId = 5;
            var executionContext = new NaheulbookExecutionContext();
            var request = new CharacterAddJobRequest {JobId = jobId};

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
                .Returns((Character) null);

            Func<Task> act = () => _service.AddJobAsync(executionContext, characterId, request);

            act.Should().Throw<CharacterNotFoundException>();
        }

        [Test]
        public void AddJobAsync_EnsureCanAccessCharacter()
        {
            var jobId = Guid.NewGuid();
            const int characterId = 5;
            var executionContext = new NaheulbookExecutionContext();
            var request = new CharacterAddJobRequest {JobId = jobId};
            var character = new Character {Jobs = new List<CharacterJob>(), Id = characterId};

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
                .Returns(character);

            _authorizationUtil.When(x => x.EnsureCharacterAccess(executionContext, character))
                .Throw(new TestException());

            Func<Task> act = () => _service.AddJobAsync(executionContext, characterId, request);

            act.Should().Throw<TestException>();
        }

        [Test]
        public void AddJobAsync_WhenCharacterAlreadyKnowTheJob_Throw()
        {
            const int characterId = 5;
            var jobId = Guid.NewGuid();
            var executionContext = new NaheulbookExecutionContext();
            var request = new CharacterAddJobRequest {JobId = jobId};
            var character = new Character {Jobs = new List<CharacterJob> {new CharacterJob {JobId = jobId}}, Id = characterId};

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
                .Returns(character);

            Func<Task> act = () => _service.AddJobAsync(executionContext, characterId, request);

            act.Should().Throw<CharacterAlreadyKnowThisJobException>();
        }

        [Test]
        public void AddJobAsync_WhenJobNotFoundThrow()
        {
            const int characterId = 5;
            var jobId = Guid.NewGuid();
            var executionContext = new NaheulbookExecutionContext();
            var request = new CharacterAddJobRequest {JobId = jobId};
            var character = new Character {Jobs = new List<CharacterJob>(), Id = characterId};

            _unitOfWorkFactory.GetUnitOfWork().Characters.GetWithOriginWithJobsAsync(characterId)
                .Returns(character);
            _unitOfWorkFactory.GetUnitOfWork().Jobs.GetAsync(jobId)
                .Returns((Job) null);

            Func<Task> act = () => _service.AddJobAsync(executionContext, characterId, request);

            act.Should().Throw<JobNotFoundException>();
        }
    }
}