using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class CharactersControllerTest
    {
        private ICharacterService _characterService;
        private IMapper _mapper;
        private NaheulbookExecutionContext _executionContext;

        private CharactersController _controller;
        private ICharacterBackupService _characterBackupService;

        [SetUp]
        public void SetUp()
        {
            _characterService = Substitute.For<ICharacterService>();
            _mapper = Substitute.For<IMapper>();
            _characterBackupService = Substitute.For<ICharacterBackupService>();

            _controller = new CharactersController(
                _characterService,
                _mapper,
                _characterBackupService
            );

            _executionContext = new NaheulbookExecutionContext();
        }

        [Test]
        public async Task GetCharactersListAsync_ShouldLoadListOfCharacterFromService_ThenMapItIntoSummary()
        {
            var characters = new List<CharacterEntity>();
            var charactersResponse = new List<CharacterSummaryResponse>();

            _characterService.GetCharacterListAsync(_executionContext)
                .Returns(characters);
            _mapper.Map<List<CharacterSummaryResponse>>(characters)
                .Returns(charactersResponse);

            var result = await _controller.GetCharactersListAsync(_executionContext);

            result.Value.Should().BeSameAs(charactersResponse);
        }

        [Test]
        public async Task GetCharacterDetailsAsync_ShouldLoadCharacterFromService_ThenMapItIntoResponse()
        {
            const int characterId = 2;
            const int groupMasterId = 3;
            var character = new CharacterEntity {Group = new GroupEntity {MasterId = groupMasterId}};
            var characterResponse = new CharacterResponse();

            _characterService.LoadCharacterDetailsAsync(_executionContext, characterId)
                .Returns(character);
            _mapper.Map<CharacterResponse>(character)
                .Returns(characterResponse);

            var result = await _controller.GetCharacterDetailsAsync(_executionContext, characterId);

            result.Value.Should().BeSameAs(characterResponse);
        }

        [Test]
        public async Task GetCharacterDetailsAsync_ShouldLoadCharacterFromService_ThenMapItIntoCharacterResponse_WhenNoGroup()
        {
            const int characterId = 2;
            var character = new CharacterEntity();
            var characterResponse = new CharacterResponse();

            _characterService.LoadCharacterDetailsAsync(_executionContext, characterId)
                .Returns(character);
            _mapper.Map<CharacterResponse>(character)
                .Returns(characterResponse);

            var result = await _controller.GetCharacterDetailsAsync(_executionContext, characterId);

            result.Value.Should().BeSameAs(characterResponse);
        }

        [Test]
        public async Task GetCharacterDetailsAsync_ShouldLoadCharacterFromService_ThenMapItIntoCharacterForGmResponse()
        {
            const int characterId = 2;
            const int groupMasterId = 3;
            _executionContext.UserId = groupMasterId;
            var character = new CharacterEntity {Group = new GroupEntity {MasterId = groupMasterId}};
            var characterResponse = new CharacterFoGmResponse();

            _characterService.LoadCharacterDetailsAsync(_executionContext, characterId)
                .Returns(character);
            _mapper.Map<CharacterFoGmResponse>(character)
                .Returns(characterResponse);

            var result = await _controller.GetCharacterDetailsAsync(_executionContext, characterId);

            result.Value.Should().BeSameAs(characterResponse);
        }

        [Test]
        public async Task GetCharacterDetailsAsync_WhenCatchForbiddenAccessException_Return403()
        {
            const int characterId = 2;
            _characterService.LoadCharacterDetailsAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>())
                .Returns(Task.FromException<CharacterEntity>(new ForbiddenAccessException()));

            Func<Task> act = () => _controller.GetCharacterDetailsAsync(_executionContext, characterId);

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }

        [Test]
        public async Task GetCharacterDetailsAsync_WhenCatchCharacterNotFoundException_Return404()
        {
            const int characterId = 2;
            _characterService.LoadCharacterDetailsAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>())
                .Returns(Task.FromException<CharacterEntity>(new CharacterNotFoundException(characterId)));

            Func<Task> act = () => _controller.GetCharacterDetailsAsync(_executionContext, characterId);

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Test]
        public async Task PostCreateCharacterAsync_ShouldCreateCharacterWithCharacterService_ThenReturnCharacterResponse()
        {
            var createCharacterRequest = new CreateCharacterRequest();
            var createdCharacter = new CharacterEntity();
            var characterResponse = new CreateCharacterResponse();

            _characterService.CreateCharacterAsync(_executionContext, createCharacterRequest)
                .Returns(createdCharacter);
            _mapper.Map<CreateCharacterResponse>(createdCharacter)
                .Returns(characterResponse);

            var result = await _controller.PostCreateCharacterAsync(_executionContext, createCharacterRequest);

            result.Value.Should().BeSameAs(characterResponse);
            result.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Test]
        public async Task PostAddItemToCharacterInventory_ShouldLoadCharacterFromService_ThenMapItIntoResponse()
        {
            const int characterId = 2;
            var item = new ItemEntity();
            var itemResponse = new ItemResponse();
            var request = new CreateItemRequest();

            _characterService.AddItemToCharacterAsync(_executionContext, characterId, request)
                .Returns(item);
            _mapper.Map<ItemResponse>(item)
                .Returns(itemResponse);

            var result = await _controller.PostAddItemToCharacterInventoryAsync(_executionContext, characterId, request);

            result.Value.Should().BeSameAs(itemResponse);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public async Task PostAddItemToCharacterInventory_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            const int characterId = 2;
            var request = new CreateItemRequest();

            _characterService.AddItemToCharacterAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<CreateItemRequest>())
                .Returns(Task.FromException<ItemEntity>(exception));

            Func<Task> act = () => _controller.PostAddItemToCharacterInventoryAsync(_executionContext, characterId, request);

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        public async Task PostAddItemToCharacterInventory_WhenCatchItemTemplateNotFound_Return400()
        {
            const int characterId = 2;
            var itemTemplateId = Guid.NewGuid();
            var request = new CreateItemRequest();

            _characterService.AddItemToCharacterAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<CreateItemRequest>())
                .Returns(Task.FromException<ItemEntity>(new ItemTemplateNotFoundException(itemTemplateId)));

            Func<Task> act = () => _controller.PostAddItemToCharacterInventoryAsync(_executionContext, characterId, request);

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Test]
        public async Task GetCharacterLoots_ShouldLoadCharacterLootsFromService_ThenMapItIntoResponse()
        {
            const int characterId = 2;
            var loots = new List<LootEntity>();
            var lootsResponse = new List<LootResponse>();

            _characterService.GetCharacterLootsAsync(_executionContext, characterId)
                .Returns(loots);
            _mapper.Map<List<LootResponse>>(loots)
                .Returns(lootsResponse);

            var result = await _controller.GetCharacterLootsAsync(_executionContext, characterId);

            result.Value.Should().BeSameAs(lootsResponse);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public async Task GetCharacterLoots_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            _characterService.GetCharacterLootsAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>())
                .Returns(Task.FromException<List<LootEntity>>(exception));

            Func<Task> act = () => _controller.GetCharacterLootsAsync(_executionContext, 2);

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public async Task PostAddModifiersAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            _characterService.AddModifiersAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<AddCharacterModifierRequest>())
                .Returns(Task.FromException<CharacterModifierEntity>(exception));

            Func<Task> act = () => _controller.PostAddModifiersAsync(_executionContext, 2, new AddCharacterModifierRequest());

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterModifierExceptionsAndExpectedStatusCode))]
        public async Task DeleteModifiersAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            _characterService.DeleteModifiersAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(Task.FromException<CharacterModifierEntity>(exception));

            Func<Task> act = () => _controller.DeleteModifiersAsync(_executionContext, 2, 4);

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetToggleCharacterModifierExceptionsAndExpectedStatusCode))]
        public async Task PostToggleModifiersAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            _characterService.ToggleModifiersAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(Task.FromException<CharacterModifierEntity>(exception));

            Func<Task> act = () => _controller.PostToggleModifiersAsync(_executionContext, 2, 4);

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        public async Task GetCharacterHistoryEntryAsyncShouldLoadCharacterLootsFromService_ThenMapItIntoResponse()
        {
            const int characterId = 2;
            var historyEntries = new List<IHistoryEntry>();
            var historyEntriesResponse = new List<IHistoryEntryResponse>();

            _characterService.GetCharacterHistoryEntryAsync(_executionContext, characterId, 8)
                .Returns(historyEntries);
            _mapper.Map<List<IHistoryEntryResponse>>(historyEntries)
                .Returns(historyEntriesResponse);

            var result = await _controller.GetCharacterHistoryEntryAsync(_executionContext, characterId, 8);

            result.Value.Should().BeSameAs(historyEntriesResponse);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public async Task GetCharacterHistoryEntryAsyncShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            _characterService.GetCharacterHistoryEntryAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(Task.FromException<List<IHistoryEntry>>(exception));

            Func<Task> act = () => _controller.GetCharacterHistoryEntryAsync(_executionContext, 42, 0);

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
        }


        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public async Task PatchCharacterAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            _characterService.UpdateCharacterAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<PatchCharacterRequest>())
                .Returns(Task.FromException<List<LootEntity>>(exception));

            Func<Task> act = () => _controller.PatchCharacterAsync(_executionContext, 2, new PatchCharacterRequest());

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        [TestCaseSource(nameof(GetCommonCharacterExceptionsAndExpectedStatusCode))]
        public async Task PutStatBonusAdAsync_ShouldReturnExpectedHttpStatusCodeOnKnownErrors(Exception exception, int expectedStatusCode)
        {
            _characterService.SetCharacterAdBonusStatAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<PutStatBonusAdRequest>())
                .Returns(Task.FromException<List<LootEntity>>(exception));

            Func<Task> act = () => _controller.PutStatBonusAdAsync(_executionContext, 2, new PutStatBonusAdRequest());

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(expectedStatusCode);
        }

        private static IEnumerable<TestCaseData> GetCommonCharacterExceptionsAndExpectedStatusCode()
        {
            yield return new TestCaseData(new ForbiddenAccessException(), StatusCodes.Status403Forbidden);
            yield return new TestCaseData(new CharacterNotFoundException(42), StatusCodes.Status404NotFound);
        }

        private static IEnumerable<TestCaseData> GetCommonCharacterModifierExceptionsAndExpectedStatusCode()
        {
            foreach (var testCaseData in GetCommonCharacterExceptionsAndExpectedStatusCode())
                yield return testCaseData;
            yield return new TestCaseData(new CharacterModifierNotFoundException(42), StatusCodes.Status404NotFound);
        }

        private static IEnumerable<TestCaseData> GetToggleCharacterModifierExceptionsAndExpectedStatusCode()
        {
            foreach (var testCaseData in GetCommonCharacterModifierExceptionsAndExpectedStatusCode())
                yield return testCaseData;
            yield return new TestCaseData(new CharacterModifierNotReusableException(42), StatusCodes.Status400BadRequest);
        }
    }
}